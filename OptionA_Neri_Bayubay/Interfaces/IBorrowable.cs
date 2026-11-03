namespace OptionA_Neri_Bayubay.Interfaces
{
    public interface IBorrowable
    {
        string AssetTag { get; }
        string BorrowerId { get; }
        bool IsCheckedOut { get; }

        void CheckOut(string borrowerId);
        void Return();
    }
}
