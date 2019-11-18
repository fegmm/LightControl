using FTD2XX_NET;
using System.Threading;
using System.Threading.Tasks;
using static FTD2XX_NET.FTDI;

namespace LightControl.Services
{
    public class DMXService
    {
        readonly FTDI ftdi;
        readonly byte[] buffer;

        public bool IsConnected { get; private set; }

        public DMXService()
        {
            ftdi = new FTDI();
            buffer = new byte[513];
        }

        public bool Connect()
        {
            FT_STATUS status;
            bool result;
            uint numberOfDevices = 0;
            ftdi.GetNumberOfDevices(ref numberOfDevices);

            if (numberOfDevices == 0)
                result = false;
            else if ((status = ftdi.OpenByIndex(0)) != FT_STATUS.FT_OK)
                result = false;
            else if ((status = ftdi.SetBaudRate(250000)) != FT_STATUS.FT_OK)
                result = false;
            else if ((status = ftdi.SetDataCharacteristics(FT_DATA_BITS.FT_BITS_8, FT_STOP_BITS.FT_STOP_BITS_2, FT_PARITY.FT_PARITY_NONE)) != FT_STATUS.FT_OK)
                result = false;
            else
            {
                status = ftdi.Purge(FT_PURGE.FT_PURGE_TX | FT_PURGE.FT_PURGE_RX);
                IsConnected = true;
                result = true;
            }

            return result;
        }

        public bool Disconnect()
        {
            IsConnected = false;
            return ftdi.Close() == FT_STATUS.FT_OK;
        }

        public bool SendToDevice(byte[] buffer)
        {
            ftdi.SetBreak(true);
            ftdi.SetBreak(false);
            uint written = 0;
            FT_STATUS writeStatus;
            writeStatus = ftdi.Write(buffer, 513, ref written);
            if (writeStatus == FT_STATUS.FT_OK)
                return true;
            return false;
        }

        public void UpdatePeriodicalBuffer(int channel, byte[] data)
        {
            if (data is null)
            {
                return;
            }

            for (int i = 0; i < data.Length; i++)
            {
                buffer[channel + i] = data[i];
            }
        }

        public CancellationTokenSource StartPeriodicalSend(int milliseconds)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            new Task(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    SendToDevice(buffer);
                    Thread.Sleep(milliseconds);
                }
            }, token, TaskCreationOptions.LongRunning).Start();
            return tokenSource;
        }
    }
}
