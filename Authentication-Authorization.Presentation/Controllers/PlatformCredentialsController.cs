using Authentication_Authorization.BLL.Contracts.Enums;
using Authentication_Authorization.BLL.Contracts.Interfaces;
using Authentication_Authorization.BLL.DTOs.PlatformCredentialsDTOs;
using Authentication_Authorization.BLL.Models;
using Authentication_Authorization.Presentation.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authentication_Authorization.Presentation.Controllers
{
    [ApiController]
    [IsAuthorized(Roles.All)]
    [Route("api/platform-credentials")]
    public class PlatformCredentialsController : ControllerBase
    {
        private readonly IPlatformCredentialsService _credentialsService;

        private readonly int userId;


        public PlatformCredentialsController(
            IPlatformCredentialsService credentialsService,
            IHttpContextAccessor context
        )
        {
            _credentialsService = credentialsService;
            userId = Convert.ToInt32(context.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "id").Value.ToString());
        }


        [HttpGet]
        public ActionResult<IEnumerable<PlatformCredentialsResponseDTO>> GetAllPlatformCredentials()
        {
            IEnumerable<PlatformCredentialsResponseDTO> allPlatformCredentials = 
                _credentialsService.GetAllCredentials(userId);

            return Ok(allPlatformCredentials);
        }

        [HttpGet("{id}")]
        public ActionResult<PlatformCredentialsResponseDTO> GetPlatformCredentialsById(int id)
        {
            PlatformCredentialsResponseDTO platformCredentialsById = 
                _credentialsService.GetCredentialsById(userId, id);

            return Ok(platformCredentialsById);
        }

        [HttpPost]
        public ActionResult<PlatformCredentialsConfirmationDTO> PostPlatformCredentials(
            [FromBody] PlatformCredentialsPostBodyDTO newCredentials
        )
        {
            PlatformCredentialsConfirmationDTO confirmation = 
                _credentialsService.AddCredentials(userId, newCredentials);

            return Created(Request.Path, confirmation);
        }

        [HttpPut("{id}")]
        public ActionResult<PlatformCredentialsConfirmationDTO> PutPlatformCredentials(
            int id, 
            [FromBody] PlatformCredentialsPutBodyDTO updatedCredentials
        )
        { 
            PlatformCredentialsConfirmationDTO confirmation = 
                _credentialsService.UpdateCredentials(userId, id, updatedCredentials);

            return Ok(confirmation);
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePlatformCredentials(int id)
        {
            _credentialsService.DeleteCredentials(userId, id);

            return NoContent();
        }

        [HttpPost("check-password/{id}")]
        public ActionResult CheckPassword(int id, [FromBody] PasswordModel passwordModel)
        {
            string credentialPassword = _credentialsService.GetPlatformPassword(id, userId, passwordModel.Password);

            return Ok(new { password = credentialPassword });
        }

        [HttpPost("report")]
        public ActionResult GetPdf([FromBody] PdfRequestModel requestModel)
        {
            byte[] file = _credentialsService.GeneratePlatformCredentialsPdf(userId, requestModel);

            return File(file, "application/pdf");
        }

        [HttpPost("{id}/image")]
        public ActionResult UploadImage(int id, IFormFile image)
        {
            _credentialsService.AddImage(id, userId, image);

            return Ok();
        }

        [HttpPut("{id}/image")]
        public ActionResult UpdateImage(int id, IFormFile image)
        {
            _credentialsService.UpdateImage(id, userId, image);

            return Ok();
        }
    }
}
