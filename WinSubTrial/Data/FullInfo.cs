using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial
{
    public class RequestInfo
    {
        public string Brand { get; set; }
        public string SDK { get; set; }
        public string Country { get; set; }
        public string Network { get; set; }
    }
    public class BaseInfo
    {
        public string BaseBand { get; set; }
        public string Board { get; set; }
        public string BOOTLOADER { get; set; }
        public string Brand { get; set; }
        public string Device { get; set; }
        public string DisplayId { get; set; }
        public string GLRenderer { get; set; }
        public string GLVendor { get; set; }
        public string Hardware { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Platform { get; set; }
        public string Product { get; set; }
    }

    public class FullInfo
    {
        public string Ssid { get; set; }
        public string Bssid { get; set; }
        public string MacAddress { get; set; }
        public string SimOperator { get; set; }
        public string SimOperatorName { get; set; }
        public string Mcc { get; set; }
        public string Mnc { get; set; }
        public string CountryCode { get; set; }
        public string IMEI { get; set; }
        public string SimSerial { get; set; }
        public string SubscriberId { get; set; }
        public string PhoneNumber { get; set; }
        public string serial { get; set; }
        public string androidID { get; set; }
        public string AndroidSeri { get; set; }
        public string GLVendor { get; set; }
        public string GLRenderer { get; set; }
        public string release { get; set; }
        public string sdk { get; set; }
        public string BaseBand { get; set; }
        public string BOOTLOADER { get; set; }
        public string platform { get; set; }
        public string hardware { get; set; }
        public string board { get; set; }
        public string wifiMac { get; set; }
        public string wifiName { get; set; }
        public string BSSID { get; set; }
        public string fingerprint { get; set; }
        public string UserAgent { get; set; }
        public string description { get; set; }
        public string incremantal { get; set; }
        public string security_patch { get; set; }
        public string BuildDate { get; set; }
        public string DateUTC { get; set; }
        public string lon { get; set; }
        public string lat { get; set; }
        public string displayId { get; set; }
        public string radio { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string id { get; set; }
        public string btype { get; set; }
        public string btags { get; set; }
        public string buser { get; set; }
        public string host { get; set; }
        
        //public string manufacturer { get; set; }
        //public string device { get; set; }
        //public string product { get; set; }
    }
}
