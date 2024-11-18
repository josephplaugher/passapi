using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soar.Interfaces;
using Soar.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Soar.Services;

public class ContactUploadService : BaseUploadService, IContactUploadService
{
    public ContactUploadService(SoarContext context) : base(context)
    {
    }

    public async Task<JsonResult> Upload(IFormFile file, string username)
    {
        try
        {
            excelHeaders.Clear();
            if (file == null || file.Length == 0)
            {
                throw new Exception("Upload a file.");
            }
            await CreateWorksheet(file);

            // loop through each row in the excel file and do the stuff
            for (int row = 2; (row - 1) < rowCount; row++)
            {
                bool contactExistsOnRow = CheckForContactOnRow(row);
                bool contactAlreadyExists = await CheckIfContactExists(row);
                if (contactExistsOnRow == true && contactAlreadyExists == false)
                {
                    await AddContact(row, username);
                    response.SuccessfulRows++;
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors.Add(ex.Message);
        }

        return new JsonResult(response);
    }

    private bool CheckForContactOnRow(int row)
    {
        string firstName = GetStringColumnValue(row, nameof(Contact.FirstName));
        string lastName = GetStringColumnValue(row, nameof(Contact.LastName));
        if (firstName == null && lastName == null)
        {
            return false;
        }
        else if (firstName != null && lastName != null)
        {
            return true;
        }
        else
        {
            throw new Exception($"Error on row {row}: Both first name and last name are required to create a contact");
        }
    }

    private async Task<bool> CheckIfContactExists(int row)
    {
        string firstName = GetStringColumnValue(row, nameof(Contact.FirstName));
        string lastName = GetStringColumnValue(row, nameof(Contact.LastName));
        string email = GetStringColumnValue(row, nameof(Contact.Email));
        Contact contactExists = await _context.Contact.Where(x => x.FirstName == firstName && x.LastName == lastName && x.Email == email).FirstOrDefaultAsync();
        if (contactExists == null)
        {
            return false;
        }
        else
        {
            response.Errors.Add($"Error, Contact on line {row} already exists. Contact ID {contactExists.ContactId}");
            return true;
        }
    }

    private async Task<int> AddContact(int row, string username)
    {
        Contact contact = new();
        string firstName = GetStringColumnValue(row, nameof(Contact.FirstName));
        string lastName = GetStringColumnValue(row, nameof(Contact.LastName));

        contact.FirstName = firstName;
        contact.LastName = lastName;
        contact.Email = GetStringColumnValue(row, nameof(Contact.Email));
        contact.Address1 = GetStringColumnValue(row, nameof(Contact.Address1));
        contact.Address2 = GetStringColumnValue(row, nameof(Contact.Address2));
        contact.City = GetStringColumnValue(row, nameof(Contact.City));
        contact.State = GetStringColumnValue(row, nameof(Contact.State));
        contact.ZipCode = GetStringColumnValue(row, nameof(Contact.ZipCode));
        contact.Phone = GetStringColumnValue(row, nameof(Contact.Phone));
        contact.Type = GetStringColumnValue(row, nameof(Contact.Type));
        contact.AddedOn = DateTime.Now;
        contact.AddedBy = username;
        contact.Source = GetStringColumnValue(row, nameof(Contact.Source));
        contact.Organization = GetStringColumnValue(row, nameof(Contact.Organization));

        await _context.Contact.AddAsync(contact);
        await _context.SaveChangesAsync();
        //get Contact with ID added
        contact = await _context.Contact.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefaultAsync();

        return 0;
    }
}