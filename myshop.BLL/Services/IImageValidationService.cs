namespace myshop.BLL.Services;

public interface IImageValidationService
{
    (bool isValid, string errorMessage) IsValid(string fileName, long fileSize);
}