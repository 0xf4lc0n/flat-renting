namespace FlatRenting.Interfaces;

public interface IPhotoService {
    Task<string> UploadPhoto(IFormFile imgFile);
}
