using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Triggerless.API.Controllers
{
    public class CustomerController: BaseController
    {

        protected long? CustomerID { get; private set; }  // available to subclasses

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            CustomerID = null;

            if (controllerContext != null && controllerContext.Request != null)
            {
                var headers = controllerContext.Request.Headers;
                var values = default(System.Collections.Generic.IEnumerable<string>);
                if (headers.TryGetValues("CustomerID", out values))
                {
                    var first = values.FirstOrDefault();
                    long parsed;
                    if (long.TryParse(first, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
                        CustomerID = parsed;
                }
            }
        }

        protected bool HasValidCustomerId()
        {
            return CustomerID.HasValue && CustomerID.Value > 0;
        }
    }

}