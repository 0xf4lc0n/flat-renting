using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FlatRenting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatRenting.Controllers;

[Route("api/[controller]")]
public class PhotoController : RestrictedApiController {
    private readonly IEmailService _email;
    private readonly IPhotoService _photo;

    public PhotoController(IEmailService email, IPhotoService photo) {
        _email = email;
        _photo = photo;
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(List<IFormFile> files) {
        var urls = new List<string>();

        foreach (var file in files) {
            urls.Add(await _photo.UploadPhoto(file));
        }

        return Ok(urls);
    }

    //public void UploadPhoto() {
    //    var uploadParams = new ImageUploadParams() {
    //        File = new FileDescription(@"photo.jpg")
    //    };

    //    //var uploadResult 
    //}
    //private void Creds() {
    //    Account account = new Account(
    //      "dbxpdrfbx",
    //      "175573919965845",
    //      "pYDupTu0yV8-ogWVanfZzf8nNA8");
    //    //CLOUDINARY_URL=cloudinary://175573919965845:pYDupTu0yV8-ogWVanfZzf8nNA8@dbxpdrfbx
    //    Cloudinary cloudinary = new Cloudinary(account);
    //}
}
