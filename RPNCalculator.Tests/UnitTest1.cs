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

            Guid guid2 = new Guid();
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

    }
}