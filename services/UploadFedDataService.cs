using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Soar.Helpers;
using Soar.Models;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.CodeAnalysis.Elfie.Model;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
//using static MsgReader.Outlook.Storage;

namespace Soar.Services;

public interface IUploadFedDataService
{
    Task<JsonResult> UploadFedData(IFormFile file, string username);
}
public class UploadFedDataService : IUploadFedDataService
{
    private readonly SoarContext _context;
    private readonly HttpClient _httpClient;
    private readonly IFileUpDownloadService _fileUpDownloadService;
    private readonly IWebHostEnvironment _env;
    private DataUploadResponse response = new();
    private readonly Dictionary<string, int> excelHeaders = [];
    private int rowCount = 0;
    private ExcelPackage package;
    private ExcelWorksheet worksheet;
    private int columnCount = 0;
    private Contact contact;
    private Communication newComm;
    private string topicsFromFile;
    private List<string> existingTopicNames;
    private List<string> possibleNewTopics;

    public UploadFedDataService(SoarContext context, HttpClient httpClient, IFileUpDownloadService fileUpDownloadService, IWebHostEnvironment env)
    {
        _context = context;
        _httpClient = httpClient;
        _fileUpDownloadService = fileUpDownloadService;
        _env = env;
    }

    public async Task<JsonResult> UploadFedData(IFormFile file, string username)
    {
        try
        {
            List<Models.Topic> existingTopics = await _context.Topic.ToListAsync();
            existingTopicNames = existingTopics.Select(t => t.Name.ToLower()).ToList();
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
            excelHeaders.Clear();
            if (file == null || file.Length == 0)
            {
                throw new Exception("Upload a file.");
            }
            await CreateWorksheet(file);

            for (int col = 1; col <= columnCount; col++)
            {
                string columnName = worksheet.Cells[1, col].Value?.ToString().Trim();
                excelHeaders.Add(columnName.Replace(" ", "").ToLower(), col);
            }
            // loop through each row in the excel file and do the stuff
            for (int row = 2; (row - 1) < rowCount; row++)
            {
                //reset variables for each row
                newComm = new();
                contact = new();
                topicsFromFile = string.Empty;
                possibleNewTopics = [];

                string sourceId = worksheet.Cells[row, 1].Value?.ToString();
                Communication duplicateComm = await _context.Communication.Where(c => c.SourceId == sourceId).FirstOrDefaultAsync();
                if (duplicateComm == null)
                {
                    bool newTopicsToAdd = CheckForTopics(row); // this method adds new topics to the database, the topics are added to the Communication object in the CreateCommunication method
                    if (newTopicsToAdd == true) { await AddNewTopics(row); }
                    await CreateCommunication(row, file);
                    bool contactExistsOnRow = CheckForContactOnRow(row);
                    if (contactExistsOnRow == true)
                    {
                        bool contactAlreadyExists = await CheckIfContactExists(row);
                        if (contactAlreadyExists == false) { await AddContact(row); }
                        CreateContactCommunicationRelationship();
                    }

                    bool attachmentsExist = CheckForAttachments(row);
                    if (attachmentsExist == true)
                    {
                        List<Models.Attachment> downloadedFiles = AddAttachments(row, file, sourceId, username).Result;
                        downloadedFiles.ForEach(a => newComm.Attachment.Add(a));
                    }

                    await _context.SaveChangesAsync();
                    response.SuccessfulRows++;
                }
                else
                {
                    response.FailedRows++;
                    response.Errors.Add(sourceId + " already exists in the database");
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors.Add(ex.Message);
        }
        return new JsonResult(response);
    }

    private async Task<int> CreateWorksheet(IFormFile file)
    {
        using MemoryStream stream = new();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        package = new(stream);
        worksheet = package.Workbook.Worksheets[0];

        rowCount = worksheet.Dimension.Rows;
        columnCount = worksheet.Dimension.Columns;
        response.TotalRowsProcessed = rowCount - 1; //minus 1 accounts for header row

        return 0;
    }

    private bool CheckForContactOnRow(int row)
    {
        string firstName = worksheet.Cells[row, 41].Value?.ToString();
        string lastName = worksheet.Cells[row, 42].Value?.ToString();
        if (firstName == null && lastName == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private async Task<bool> CheckIfContactExists(int row)
    {
        string firstName = worksheet.Cells[row, 41].Value?.ToString();
        string lastName = worksheet.Cells[row, 42].Value?.ToString();
        Contact contactExists = await _context.Contact.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefaultAsync();
        if (contactExists == null)
        {
            return false;
        }
        else
        {
            // if the contact exists, assign it to the contact property
            contact = contactExists;
            return true;
        }
    }

    private async Task<int> AddContact(int row)
    {
        string firstName = worksheet.Cells[row, 41].Value?.ToString();
        string lastName = worksheet.Cells[row, 42].Value?.ToString();

        contact.FirstName = firstName;
        contact.LastName = lastName;
        contact.City = worksheet.Cells[row, 43].Value?.ToString();
        contact.State = worksheet.Cells[row, 44].Value?.ToString();
        contact.ZipCode = worksheet.Cells[row, 45].Value?.ToString();
        contact.AddedOn = DateTime.Now;
        contact.AddedBy = "System";
        contact.Organization = worksheet.Cells[row, 47].Value?.ToString();
        contact.Source = "Federal Register";

        await _context.Contact.AddAsync(contact);
        await _context.SaveChangesAsync();
        //get Contact with ID added
        contact = await _context.Contact.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefaultAsync();

        return 0;
    }

    public async Task<int> CreateCommunication(int row, IFormFile file)
    {
        string dateString = worksheet.Cells[row, 6].Value?.ToString();
        if (dateString != null)
        {
            DateTime date = DateTime.Parse(dateString);
            newComm.DateSubmitted = date;
        }
        newComm.Status = "Not Started";
        newComm.CommunicationSource = "Federal Register";
        newComm.RawText = worksheet.Cells[row, 53].Value?.ToString();
        newComm.AddedOn = DateTime.Now;
        newComm.AddedBy = "System";
        string SourceId = worksheet.Cells[row, 1].Value?.ToString();
        newComm.SourceId = SourceId;
        newComm.Title = worksheet.Cells[row, 10].Value?.ToString();
        newComm.ImportDate = DateTime.Now;
        newComm.ImportSource = file.FileName;
        newComm.Topic = topicsFromFile;

        await _context.Communication.AddAsync(newComm);
        await _context.SaveChangesAsync();
        // the Communcation with the ID
        newComm = await _context.Communication.Where(x => x.SourceId == SourceId).FirstOrDefaultAsync();

        return 0;
    }

    private int CreateContactCommunicationRelationship()
    {
        ContactCommunication contactCommunication = new ContactCommunication()
        {
            Contact = contact,
            ContactId = contact.ContactId,
            Communication = newComm,
            CommunicationId = newComm.CommunicationId
        };
        newComm.Contacts.Add(contactCommunication);

        return 0;
    }

    private bool CheckForAttachments(int row)
    {
        string attachments = excelHeaders.TryGetValue("attachmentfiles", out int columnNumber) ? worksheet.Cells[row, columnNumber].Value?.ToString() : "";
        if (attachments != string.Empty && attachments != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<List<Models.Attachment>> AddAttachments(int row, IFormFile file, string sourceId, string username)
    {
        List<Models.Attachment> attachmentsList = new();
        string attachments = excelHeaders.TryGetValue("attachmentfiles", out int columnNumber) ? worksheet.Cells[row, columnNumber].Value?.ToString() : "";
        // split comma separated url list
        string[] attachmentList = attachments.Split(",");
        for (var i = 0; i < attachmentList.Length; i++)
        {
            try
            {
                var httpresponse = await _httpClient.GetAsync(attachmentList[i]);
                if (httpresponse.IsSuccessStatusCode)
                {
                    byte[] fileContent = await httpresponse.Content.ReadAsByteArrayAsync();
                    var contentType = httpresponse.Content.Headers.ContentType?.MediaType.ToString() ?? "application/octet-stream";
                    var fileName = $"{sourceId}-attachment{i}.{contentType.Split("/")[1]}";

                    // Get the downloaded file, convert it to IFormFile, and then save it to the local file system
                    using var stream = new MemoryStream(fileContent);
                    IFormFile convertedFile = new FormFile(stream, 0, stream.Length, "", fileName);

                    Models.Attachment a = await _fileUpDownloadService.SaveToFileSystem(convertedFile, contentType, fileName, username, newComm);
                    attachmentsList.Add(a);
                    await _context.Attachment.AddAsync(a);
                }
                else
                {
                    throw new Exception($"attachment from url {attachmentList[i]} for source id {sourceId} download failed");
                }

            }
            catch (Exception ex)
            {
                response.Errors.Add($"Error downloading file {attachmentList[i]}: {ex.Message}. {ex.InnerException}");
            }
        }
        return attachmentsList;
    }

    private bool CheckForTopics(int row)
    {
        topicsFromFile = worksheet.Cells[row, 31].Value?.ToString();
        if(topicsFromFile == string.Empty || topicsFromFile == null)
        {
            return false;
        }
        possibleNewTopics = topicsFromFile.Split(";").ToList();

        bool allIncluded = possibleNewTopics.All(ct => existingTopicNames.Contains(ct.ToLower()));
        // this line checks if all the topics in the file are already in the database
        // if it's true, then there are no new topics to add, so we return its opposite
        return !allIncluded;
    }

    private async Task<int> AddNewTopics(int row)
    {
        List<Models.Topic> topicsToAdd = [];

        // Get the list of topics from the current row in the file
        List<string> newTopics = possibleNewTopics.Where(ct => !existingTopicNames.Contains(ct)).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        // look through the list of topics
        newTopics.ForEach((nt) =>
        {
            Models.Topic t = new Models.Topic();
            // Split the current topic into its separate words for each word to be capitalized next
            string[] newTopicPhrase = nt.Split(" ");
            string Capitalized = "";
            for (var i = 0; i < newTopicPhrase.Length; i++)
            {
                // check for a null topic first
                if (newTopicPhrase[i] != string.Empty)
                {
                    // capitalize the first letter of each word
                    Capitalized += char.ToUpper(newTopicPhrase[i][0]) + newTopicPhrase[i].Substring(1);
                    // add a space between words, unless this is the last word in the phrase
                    var wordCount = i + 1;
                    if (wordCount < newTopicPhrase.Length) Capitalized += " ";
                }
            };
            // Add the proper case Topic to the list of Topics to be saved
            t.Name = Capitalized;
            topicsToAdd.Add(t);
        });

        topicsToAdd = topicsToAdd.DistinctBy(i => i.Name).ToList();
        await _context.Topic.AddRangeAsync(topicsToAdd);

        return 0;
    }
}
