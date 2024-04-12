namespace ContractTesting_Producer.Services
{
    public interface IHttpClientService
    {
        Task<TResponse?> SendGetAsync<TResponse>(string url);
    }
}
