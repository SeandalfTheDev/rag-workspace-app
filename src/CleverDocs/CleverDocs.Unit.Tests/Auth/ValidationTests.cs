using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Application.Auth.Validation;
using CleverDocs.Unit.Tests.Helpers;
using FluentValidation.TestHelper;

namespace CleverDocs.Unit.Tests.Auth;

[TestFixture]
public class ValidationTests
{
    private LoginUserDtoValidator _loginValidator;
    private RegisterUserDtoValidator _registerValidator;

    [SetUp]
    public void SetUp()
    {
        _loginValidator = new LoginUserDtoValidator();
        _registerValidator = new RegisterUserDtoValidator();
    }

    [TestFixture]
    public class LoginValidationTests : ValidationTests
    {
        [Test]
        public void LoginValidator_WithValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var loginDto = ValidationTestHelpers.ValidLoginDto();

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void LoginValidator_WithEmptyEmail_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "", Password = "ValidPass123" };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void LoginValidator_WithInvalidEmailFormat_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "invalid-email", Password = "ValidPass123" };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void LoginValidator_WithEmailTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = ValidationTestHelpers.LoginWithLongEmail();

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void LoginValidator_WithEmptyPassword_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "test@example.com", Password = "" };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void LoginValidator_WithPasswordTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "test@example.com", Password = "12345" };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void LoginValidator_WithPasswordTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var loginDto = ValidationTestHelpers.LoginWithLongPassword();

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void LoginValidator_WithValidEmailAndMinimumPassword_ShouldBeValid()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "test@example.com", Password = "123456" };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void LoginValidator_WithValidEmailAndMaximumPassword_ShouldBeValid()
        {
            // Arrange
            var loginDto = new LoginUserDto { Email = "test@example.com", Password = new string('a', 100) };

            // Act
            var result = _loginValidator.TestValidate(loginDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [TestFixture]
    public class RegisterValidationTests : ValidationTests
    {
        [Test]
        public void RegisterValidator_WithValidData_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void RegisterValidator_WithEmptyEmail_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { Email = "" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void RegisterValidator_WithInvalidEmailFormat_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { Email = "invalid-email" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void RegisterValidator_WithEmailTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.RegisterWithLongFields();

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void RegisterValidator_WithEmptyFirstName_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { FirstName = "" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Test]
        public void RegisterValidator_WithFirstNameTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { FirstName = "A" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Test]
        public void RegisterValidator_WithFirstNameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { FirstName = new string('a', 101) };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Test]
        public void RegisterValidator_WithEmptyLastName_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { LastName = "" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void RegisterValidator_WithLastNameTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { LastName = "B" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void RegisterValidator_WithLastNameTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { LastName = new string('b', 101) };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Test]
        public void RegisterValidator_WithEmptyPassword_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { Password = "" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void RegisterValidator_WithPasswordTooShort_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { Password = "12345" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void RegisterValidator_WithPasswordTooLong_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { Password = new string('c', 101) };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void RegisterValidator_WithEmptyConfirmPassword_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.ValidRegisterDto();
            registerDto = registerDto with { ConfirmPassword = "" };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Test]
        public void RegisterValidator_WithMismatchedPasswords_ShouldHaveValidationError()
        {
            // Arrange
            var registerDto = ValidationTestHelpers.RegisterWithMismatchedPasswords();

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
                .WithErrorMessage("Passwords do not match");
        }

        [Test]
        public void RegisterValidator_WithMinimumValidLengths_ShouldBeValid()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "a@b.co",
                FirstName = "Ab",
                LastName = "Cd",
                Password = "123456",
                ConfirmPassword = "123456"
            };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void RegisterValidator_WithMaximumValidLengths_ShouldBeValid()
        {
            // Arrange
            var password = new string('a', 100);
            var registerDto = new RegisterUserDto
            {
                Email = new string('a', 245) + "@test.com", // 255 chars total
                FirstName = new string('b', 100),
                LastName = new string('c', 100),
                Password = password,
                ConfirmPassword = password
            };

            // Act
            var result = _registerValidator.TestValidate(registerDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}