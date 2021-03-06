using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;

namespace Triggerless.Services.Client
{
    public class TriggerlessApiClient
    {
        private TriggerlessApiService _service;

        public TriggerlessApiClient()
        {
            _service = new TriggerlessApiService();
        }

        public async Task<ImvuUser> GetUser(long userId)
        {
            return await _service.GetPoco<ImvuUser>($"user/{userId}");
        }

        public async Task<ImvuUser> GetUserByName(string userName)
        {
            return await _service.GetPoco<ImvuUser>($"username/{userName}");
        }
    }
}
