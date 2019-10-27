using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerApi.Controllers {
   public class DeviceFlowController : ControllerBase {
        private readonly IDeviceFlowStore _store;

        public DeviceFlowController(IDeviceFlowStore store)
        {
            _store = store;
        }

        [HttpPost("api/flow/codes")]
        public async Task StoreDeviceCode(string deviceCode, string userCode, [FromBody] DeviceCode data) {
            await _store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
        }

        [HttpGet("api/flow/codes")]
        public async Task<ActionResult> FindByCode(string userCode = null, string deviceCode = null) {
            if(string.IsNullOrWhiteSpace(userCode) &&
               string.IsNullOrWhiteSpace(deviceCode)) {
                return StatusCode(403);
            }

            if(!string.IsNullOrWhiteSpace(userCode)) {
                return Ok(await _store.FindByUserCodeAsync(userCode));
            }

            return Ok(await _store.FindByDeviceCodeAsync(deviceCode));
        }

        [HttpPut("api/flow/codes/{userCode}")]
        public async Task UpdateByUserCode(string userCode, [FromBody] DeviceCode data) {
            await _store.UpdateByUserCodeAsync(userCode, data);
        }

        [HttpDelete("api/flow/codes/{deviceCode}")]
        public async Task RemoveByDeviceCode(string deviceCode) {
            await _store.RemoveByDeviceCodeAsync(deviceCode);
        }
   } 
}