using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WiredUpWebApi.Data;

namespace WiredUpWebApi.Controllers
{
    public class ProjectsController : BaseApiController
    {
        public ProjectsController()
            : base()
        {
        }

        public ProjectsController(IUnitOfWorkData db)
            : base(db)
        {
        }
    }
}