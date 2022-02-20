using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer.UpdateContainers;

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

        public override Task OnBeginAskAsync(IUpdater updater, User askingFrom, string propertyName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override Task OnSuccessAsync(RawContainer? container, User askingFrom, string propertyName, OnSuccessContext onSuccessContext, CancellationToken cancellationToken)
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
            var _formFiller = new FormFiller<MyForm>();

            var values = new Queue<(bool, string?)>();
            values.Enqueue((false, firstName));
            values.Enqueue((false, lastName));
            values.Enqueue((false, age.ToString()));

            _formFiller.InPlaceValidate(values, out var _);

            Assert.AreEqual(_formFiller.Form.FirstName, firstName);
            Assert.AreEqual(_formFiller.Form.LastName, lastName);
            Assert.AreEqual(_formFiller.Form.Age, age);
        }

        [TestMethod]
        [DataRow("Arash", "Felani", new[] { "kjgjk", "ihj", "20" })]
        [DataRow("Arash", null, new[] { "kjgjk", "20" })]
        public void Age_Should_Be_Ok_Before_3_Tries(
            string firstName, string? lastName, string[] ageValues)
        {
            var _formFiller = new FormFiller<MyForm>();

            var values = new Queue<(bool, string?)>();
            values.Enqueue((false, firstName));
            values.Enqueue((false, lastName));

            foreach (var ageValue in ageValues)
            {
                values.Enqueue((false, ageValue));
            }

            _formFiller.InPlaceValidate(values, out var _);

            Assert.AreEqual(_formFiller.Form.FirstName, firstName);
            Assert.AreEqual(_formFiller.Form.LastName, lastName);

            Assert.AreEqual(_formFiller.Form.Age, int.Parse(ageValues[^1]));
        }

        [TestMethod]
        [DataRow("Arash", "Felani", new[] { "kjgjk", "ihj", "dfd" })]
        public void Age_Should_Be_Default_After_3_Tries(
            string firstName, string? lastName, string[] ageValues)
        {
            var _formFiller = new FormFiller<MyForm>();

            var values = new Queue<(bool, string?)>();
            values.Enqueue((false, firstName));
            values.Enqueue((false, lastName));

            foreach (var ageValue in ageValues)
            {
                values.Enqueue((false, ageValue));
            }

            _formFiller.InPlaceValidate(values, out var _);

            Assert.AreEqual(_formFiller.Form.FirstName, firstName);
            Assert.AreEqual(_formFiller.Form.LastName, lastName);

            Assert.AreEqual(_formFiller.Form.Age, default);
        }
    }
}