namespace InventoryDashboard.Infrastructure.Validations
{
    using System;
    using FluentValidation;
    using Microsoft.Extensions.DependencyInjection;

    public class Validator
    {
        private readonly IServiceProvider serviceProvider;

        public Validator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Validator()
        {
        }

        public virtual void ValidateAndThrow<T>(T request)
        {
            var validator = this.serviceProvider.GetRequiredService<IValidator<T>>();
            try
            {
                validator.ValidateAndThrow(request);
            }
            catch (ValidationException ex)
            {
                throw new Exceptions.ValidationException(string.Join(". ", ex.Errors));
            }
        }
    }
}