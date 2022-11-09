using CoreWebApplicationWithSwagger.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RPNCalculator.Tests
{
    public class Tests
    {
        private RpnCalculatorController rpnCalculatorController;

        [SetUp]
        public void Setup()
        {
            rpnCalculatorController = new RpnCalculatorController();
        }

        [Test]
        public void TestPushValueRoute()
        {
            var actionResultCreate = rpnCalculatorController.CreateStack();
            Assert.IsTrue(actionResultCreate.Result is CreatedAtActionResult);
            var createdAtActionResult = (CreatedAtActionResult)actionResultCreate.Result;
            Guid guid = (Guid)createdAtActionResult.Value;
            Assert.AreNotEqual(guid, Guid.Empty);

            var actionResultPush = rpnCalculatorController.PushIntoStack(guid, 8);
            Assert.IsTrue(actionResultPush.Result is OkObjectResult);

            var actionResultGet = rpnCalculatorController.GetStack(guid);
            Assert.IsTrue(actionResultGet.Result is OkObjectResult);

            Guid guid2 = Guid.NewGuid();
            actionResultGet = rpnCalculatorController.GetStack(guid2);
            Assert.IsTrue(actionResultGet.Result is NotFoundResult);
        }

        [Test]
        public void TestApplyOperandToStack()
        {
            var actionResultCreate = rpnCalculatorController.CreateStack();
            Assert.IsTrue(actionResultCreate.Result is CreatedAtActionResult);
            var createdAtActionResult = (CreatedAtActionResult)actionResultCreate.Result;
            Guid guid = (Guid)createdAtActionResult.Value;

            rpnCalculatorController.PushIntoStack(guid, 16);
            rpnCalculatorController.PushIntoStack(guid, 8);

            var actionResult = rpnCalculatorController.ApplyOperandToStack(RpnCalculatorController.DIVISION_IN_URL, guid);
            Assert.IsTrue(actionResult.Result is OkObjectResult);

            var objectResult = (OkObjectResult)actionResult.Result;
            var value = (Stack<double>)objectResult.Value;
            Assert.AreEqual(value.Pop(), 2);
        }

        [Test]
        public void TestDeleteStack()
        {
            Guid guid = Guid.NewGuid();
            var actionResult = rpnCalculatorController.DeleteStack(guid);
            Assert.IsTrue(actionResult is NotFoundResult);

            var actionResultCreate = rpnCalculatorController.CreateStack();
            Assert.IsTrue(actionResultCreate.Result is CreatedAtActionResult);
            var createdAtActionResult = (CreatedAtActionResult)actionResultCreate.Result;
            Guid guid2 = (Guid)createdAtActionResult.Value;

            actionResult = rpnCalculatorController.DeleteStack(guid2);
            Assert.IsTrue(actionResult is OkResult);
        }
    }
}