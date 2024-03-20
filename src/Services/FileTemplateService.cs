using generate_idp_token.Interfaces;

namespace generate_idp_token.Services;

public class FileTemplateService : IFileTemplateService
{
    public FileTemplateService()
    {

    }

    public string GetTemplateContent()
    {
        //return File.ReadAllText("Template.MassMail.html");
        //TODO remove this demo service
        return "This is a template";
    }
}
