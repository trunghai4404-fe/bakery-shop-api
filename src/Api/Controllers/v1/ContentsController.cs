using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;
using Contents.Application.Features.Files.Commands;

namespace Api.Controllers.v1;

[Route("api/v1/contents")]
public class ContentsController(ISender sender) : ApiControllerBase
{
    [HttpPost("files")]
    public async Task<IActionResult> UploadFile([FromForm] List<IFormFile> files, CancellationToken ct)
    {
        var fileDatas = new List<FileData>();
        try
        {
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                fileDatas.Add(new FileData(stream, file.FileName, file.ContentType));
            }

            var command = new UploadFilesCommand(fileDatas);
            var result = await sender.Send(command, ct);

            return HandleResult(result);
        }
        finally
        {
            foreach (var fileData in fileDatas)
            {
                await fileData.Stream.DisposeAsync();
            }
        }
    }
}