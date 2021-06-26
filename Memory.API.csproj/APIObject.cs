using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory.API
{
    //https://disk.yandex.ru/i/YK3dvwepDHpVrA
    public class APIObject : IDisposable
    {
        private bool disposedValue;
        private int number;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    MagicAPI.Free(number);
                disposedValue = true;
            }
        }

        ~APIObject()
         {
            Dispose(true);
         }

        public APIObject(int n)
        {
            number = n;
            MagicAPI.Allocate(number);
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
