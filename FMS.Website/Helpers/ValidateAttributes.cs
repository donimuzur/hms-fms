using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace FMS.Website.Helpers
{
    /// <summary>
    /// Customized data annotation validator for uploading file
    /// </summary>
    public class ValidateFileAttribute : ValidationAttribute
    {
        public int MaximumSize { get; set; }

        public override bool IsValid(object value)
        {
            int maxContentLength = MaximumSize * 1024; //250 Kb
            string[] allowedFileExtensions = { ".jpg", ".png" };
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            if (!allowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = "Allowed file types are: " + string.Join(", ", allowedFileExtensions);
                return false;
            }

            if (file.ContentLength > maxContentLength)
            {
                ErrorMessage = "The uploaded file is too large, maximum allowed size is : " +
                               (maxContentLength / 1024) + " KB";
                return false;
            }

            return true;
        }
    }
}