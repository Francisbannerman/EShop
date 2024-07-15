using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EShop.Infrastructure.Authentication;

public static class Extension
{
    public static IServiceCollection AddJwt(this IServiceCollection collection, IConfiguration configuration)
    {
        var options = new JwtOptions();
        configuration.GetSection("jwt").Bind(options);
        collection.Configure<JwtOptions>(x => x = options);

        collection.AddSingleton<IAuthenticationHandler, AuthenticationHandler>();
        collection.AddAuthentication().AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, ValidIssuer = options.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey))
            };
        });
        return collection;
    }
}