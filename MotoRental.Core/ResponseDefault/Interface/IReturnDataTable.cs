namespace MotoRental.Core.ResponseDefault.Interface
{
    public interface IReturnDataTable<T>
    {
        int TotalRecords { get; set; }
        int TotalPages { get; set; }
        T Data { get; set; }

    }
}
