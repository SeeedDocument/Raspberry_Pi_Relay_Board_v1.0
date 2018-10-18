using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace RPiRelayBoard
{
    class RPiRelayBoard
    {
        private I2cDevice device;
        private byte relayStatus;

        public static int RelayCount { get; } = 4;
        private static readonly byte REGISTER_ADDRESS = 0x06;

        public RPiRelayBoard(int address = 0x20)
        {
            relayStatus = 0xFF;
            FindDevice(address);
        }

        public bool OnRelay(int relay)
        {
            if (relay < 1 || relay > RelayCount)
            {
                return false;
            }

            relayStatus &= (byte)~(0x01 << (relay - 1));
            return UpdateRelays();
        }

        public bool OffRelay(int relay)
        {
            if (relay < 1 || relay > RelayCount)
            {
                return false;
            }

            relayStatus |= (byte)(0x01 << (relay - 1));
            return UpdateRelays();
        }

        public bool OnAllRelays()
        {
            relayStatus &= 0xF0;
            return UpdateRelays();
        }

        public bool OffAllRelays()
        {
            relayStatus |= 0x0F;
            return UpdateRelays();
        }

        public bool ToggleRelay(int relay)
        {
            if (relay < 1 || relay > RelayCount)
            {
                return false;
            }

            if (IsRelayOpen(relay))
            {
                return OffRelay(relay);
            }
            else
            {
                return OnRelay(relay);
            }
        }

        public bool IsRelayOpen(int relay)
        {
            if (relay < 1 || relay > RelayCount)
            {
                return false;
            }

            UpdateRelayStatus();
            return (relayStatus & (0x01 << (relay - 1))) == 0x00;
        }

        private async void FindDevice(int address)
        {
            await FindDeviceAsync(address);
        }

        private async Task FindDeviceAsync(int address)
        {
            var selector = I2cDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(selector);

            var settings = new I2cConnectionSettings(address);
            device = await I2cDevice.FromIdAsync(devices.First().Id, settings);
        }

        private bool UpdateRelays()
        {
            if (device != null)
            {
                var buffer = new byte[] { REGISTER_ADDRESS, relayStatus };
                var result = device.WritePartial(buffer).Status == I2cTransferStatus.FullTransfer;

                UpdateRelayStatus();

                return result;
            }

            return false;
        }

        private void UpdateRelayStatus()
        {
            if (device == null)
            {
                return;
            }

            var writeBuffer = new byte[] { REGISTER_ADDRESS };
            var readBuffer = new byte[1];

            if (device.WriteReadPartial(writeBuffer, readBuffer).Status == I2cTransferStatus.FullTransfer)
            {
                relayStatus = readBuffer.First();
            }
        }
    }
}
