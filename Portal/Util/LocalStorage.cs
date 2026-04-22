using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util
{
    public  class LocalStorage
    {
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        public async Task <ProtectedBrowserStorageResult<T>> GetValue<T>(string nameLocalStorage)
        {
            var test = await  ProtectedSessionStore.GetAsync<T>(key: nameLocalStorage);
            return test;
        }

            
    }
}
