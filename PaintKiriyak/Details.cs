using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Principal;
using System.Text;

public class ImageMetadata
{
    public string Author { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime DateTaken { get; set; }
    public DateTime DateAcquired { get; set; }
    public string ProgramName { get; set; }
    public string Description { get; set; }
    public string Titl { get; set; }
    public string Subject { get; set; }
    public string Tags { get; set; }
    public string Comment { get; set; }

    public ImageMetadata()
    {
        Author = GetSystemAuthor();
        CreationDate = DateTime.Now;
        DateTaken = DateTime.Now;
        DateAcquired = DateTime.Now;
        ProgramName = "Iras' Paint";
        Description = "No Description";
        Titl = "12345";
        Subject = "Smth";
        Tags = "1; 2; 3";
        Comment = "";
    }
    public ImageMetadata(string author, DateTime creationDate, DateTime dateTaken, DateTime dateAcquired,
                         string programName, string description, string titl, string subject, string tags, string comment)
    {
        Author = author;
        CreationDate = creationDate;
        DateTaken = dateTaken;
        DateAcquired = dateAcquired;
        ProgramName = programName;
        Description = description;
        Titl = titl;
        Subject = subject;
        Tags = tags;
        Comment = comment;
    }

    private string GetSystemAuthor()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        return identity.Name.Split('\\')[1];
    }

    public void SetMetadata(Bitmap image, string filename)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            image.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;

            using (Image img = Image.FromStream(stream))
            {
                SetProperty(img, 0x0320, Titl);        // Title
                SetProperty(img, 0x010e, Subject);      // Subject
                SetProperty(img, 0x9c9e, Tags);         // Tags
                SetProperty(img, 0x9286, Comment);      // Comment
                SetProperty(img, 0x013b, Author);       // Author
                SetProperty(img, 0x0132, CreationDate.ToString("yyyy:MM:dd HH:mm:ss")); // Creation Date
                SetProperty(img, 0x9003, DateTaken.ToString("yyyy:MM:dd HH:mm:ss")); // Date Taken
                SetProperty(img, 0x9004, DateAcquired.ToString("yyyy:MM:dd HH:mm:ss")); // Date Acquired
                SetProperty(img, 0x0131, ProgramName);  // Program Name

                img.Save(filename);
            }
        }
    }

    private void SetProperty(Image img, int id, string value)
    {
        foreach (PropertyItem propItem in img.PropertyItems)
        {
            if (propItem.Id == id)
            {
                propItem.Type = 2; // ASCII
                propItem.Value = Encoding.UTF8.GetBytes(value + "\0");
                propItem.Len = propItem.Value.Length;

                img.SetPropertyItem(propItem);
                return;
            }
        }

        PropertyItem newPropItem = img.PropertyItems[0];
        newPropItem.Id = id;
        newPropItem.Type = 2; // ASCII
        newPropItem.Value = Encoding.UTF8.GetBytes(value + "\0");
        newPropItem.Len = newPropItem.Value.Length;

        img.SetPropertyItem(newPropItem);
    }
}
