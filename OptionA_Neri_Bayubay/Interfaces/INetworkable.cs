namespace OptionA_Neri_Bayubay.Interfaces
{
    public interface INetworkable
    {
        string IpAddress { get; set; }
        string MacAddress { get; set; }
        bool HasNetworkIdentity();
    }
}
