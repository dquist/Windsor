﻿// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.WcfIntegration.Tests.Rest
{
	using System;
	using System.IO;
	using System.Net;
	using System.ServiceModel.Web;

	using Castle.Facilities.WcfIntegration.Rest;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using NUnit.Framework;

	[TestFixture, IntegrationTest]
	public class RestServiceFixture
	{
		[Test]
		public void CanCreateRestServiceWithBaseAddress()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<ICalculator>()
				                   .ImplementedBy<Calculator>()
				                   .DependsOn(new { number = 42 })
				                   .AsWcfService(new RestServiceModel("http://localhost:27198/UsingWindsor.svc"))
				))
			{
				using (var factory = new WebChannelFactory<ICalculator>(new Uri("http://localhost:27198/UsingWindsor.svc")))
				{
					var calculator = factory.CreateChannel();
					Assert.AreEqual(21, calculator.Multiply(3, 7));
				}
			}
		}

		[Test]
		public void CanCreateRestServiceWithEndpoint()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<ICalculator>()
				                   .ImplementedBy<Calculator>()
				                   .DependsOn(new { number = 42 })
				                   .AsWcfService(new RestServiceModel("http://localhost:27198/UsingWindsor.svc"))
				))
			{
				using (var factory = new WebChannelFactory<ICalculator>(new Uri("http://localhost:27198/UsingWindsor.svc")))
				{
					var calculator = factory.CreateChannel();
					Assert.AreEqual(8, calculator.Divide(56, 7));
				}
			}
		}

		[Test]
		public void CanCreateRestServiceWithoutInterfaceAndOpenHost()
		{
			using (new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Register(Component.For<Inventory>()
				                   .AsWcfService(new RestServiceModel("http://localhost:27198/UsingWindsor.svc")
					          )))
			{
				var request = WebRequest.Create("http://localhost:27198/UsingWindsor.svc/quantity/1234");
				var response = request.GetResponse();
				using (var reader = new BinaryReader(response.GetResponseStream()))
				{
					Assert.AreEqual(10, reader.ReadInt32());
				}
			}
		}
	}
}