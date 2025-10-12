namespace InventoryDashboard.Infrastructure.Workflows
{
    using System.Threading.Tasks;

    public abstract class AsyncStep<TRequest, TResponse>
    {
        private AsyncStep<TRequest, TResponse> next;

        public abstract Task<TResponse> ExecuteAsync(TRequest request);

        public virtual Task<TResponse> ExecuteNextAsync(TRequest request)
        {
            return this.next?.ExecuteAsync(request);
        }

        public void SetNextStep(AsyncStep<TRequest, TResponse> next)
        {
            if (this.next != null)
            {
                this.next.SetNextStep(next);
            }
            else
            {
                this.next = next;
            }
        }
    }
}