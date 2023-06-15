using ftrip.io.framework.Contexts;
using ftrip.io.framework.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Reservations
{
    public interface IReservationsQueryHelper
    {
        Task<int> CountActiveForGuest(Guid guestId, CancellationToken cancellationToken);

        Task<int> CountActiveForHost(Guid hostId, CancellationToken cancellationToken);
    }

    public class ReservationsQueryHelper : IReservationsQueryHelper
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CorrelationContext _correlationContext;

        public ReservationsQueryHelper(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            CorrelationContext correlationContext)
        {
            _client = httpClientFactory.CreateClient("BookingService");
            _httpContextAccessor = httpContextAccessor;
            _correlationContext = correlationContext;
        }

        public async Task<int> CountActiveForGuest(Guid guestId, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/reservations/active/guests/{guestId}/count");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            request.Headers.Add(CorrelationConstants.HeaderAttriute, _correlationContext.Id);

            var response = await _client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<int> CountActiveForHost(Guid hostId, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/reservations/active/hosts/{hostId}/count");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            request.Headers.Add(CorrelationConstants.HeaderAttriute, _correlationContext.Id);

            var response = await _client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return int.Parse(await response.Content.ReadAsStringAsync());
        }

        private string Token => _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
    }
}