using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.FillMyForm.Tests
{
    public class MyForm : AbstractForm
    {
        [FormProperty(Priority = 1)]
        public string? LastName { get; set; }

        [FillPropertyRetry(FillingError.ConvertingError, 2)]
        [FormProperty(Priority = 2)]
        public int Age { get; set; }

        [FillPropertyRetry(FillingError.TimeoutError, 2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        [FormProperty(Priority = 0)]
        public string FirstName { get; set; } = null!;

        public override Task OnBeginAsk<TForm>(FormFillingContext<TForm> fillterContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task OnSuccess<TForm>(FormFillingContext<TForm, OnSuccessContext> fillterContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }

    [TestClass()]
    public class FormFillerTests
    {
        [TestMethod]
        [DataRow("Arash", "Felani", 10)]
        [DataRow("Arash", null, int.MaxValue)]
        public void Should_Be_Ok_If_Values_Are_Valid(
            string firstName, string? lastName, int age)
        {
        }

        [TestMethod]
        [DataRow("Arash", "Felani", new[] { "kjgjk", "ihj", "20" })]
        [DataRow("Arash", null, new[] { "kjgjk", "20" })]
        public void Age_Should_Be_Ok_Before_3_Tries(
            string firstName, string? lastName, string[] ageValues)
        {
        }

        [TestMethod]
        [DataRow("Arash", "Felani", new[] { "kjgjk", "ihj", "dfd" })]
        public void Age_Should_Be_Default_After_3_Tries(
            string firstName, string? lastName, string[] ageValues)
        {
        }
    }
}