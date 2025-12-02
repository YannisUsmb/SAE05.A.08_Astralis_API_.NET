using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Astralis_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DisplayName("Comment")]
    public class CommentsController : CrudController<Comment, CommentDto, CommentDto, CommentCreateDto, CommentUpdateDto, int>
    {
        public CommentsController(ICommentRepository repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}