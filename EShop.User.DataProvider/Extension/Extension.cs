using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using EShop.Infrastructure.Security;

namespace EShop.User.DataProvider.Extension;

public static class Extension
{
    public static CreateUser SetPassword(this CreateUser user, IEncrypter encrypter)
    {
        var salt = encrypter.GetSalt();
        user.Password = encrypter.GetHash(user.Password, salt);
        return user;
    }

    //public static bool ValidatePassword(this UserCreated user, UserCreated savedUser, IEncrypter encrypter)
    public static bool ValidatePassword(this UserCreated user, string plainPassword, IEncrypter encrypter)

    {
        //return savedUser.Password.Equals(encrypter.GetHash(user.Password, encrypter.GetSalt()));
        
        string salt = encrypter.GetSalt(); // Get the constant salt
        string hashedPassword = encrypter.GetHash(plainPassword, salt);
        return user.Password.Equals(hashedPassword);
    }
}