﻿using System;
using System.Threading.Tasks;
using NSubstitute;
using Octokit;
using Octokit.Tests;
using Xunit;

public class DeploymentsClientTests
{
    public class TheGetAllMethod
    {
        private const string name = "name";
        private const string owner = "owner";

        [Fact]
        public async Task EnsuresNonNullArguments()
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll(null, name));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll(owner, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAll(owner, name, null));
        }

        [Fact]
        public async Task EnsuresNonEmptyArguments()
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll("", name));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll(owner, ""));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("\n\r")]
        public async Task EnsuresNonWhitespaceArguments(string whitespace)
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll(whitespace, name));
            await Assert.ThrowsAsync<ArgumentException>(() => client.GetAll(owner, whitespace));
        }

        [Fact]
        public void RequestsCorrectUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentsClient(connection);
            var expectedUrl = string.Format("repos/{0}/{1}/deployments", owner, name);

            client.GetAll(owner, name);
            connection.Received(1)
                .GetAll<Deployment>(Arg.Is<Uri>(u => u.ToString() == expectedUrl), Args.ApiOptions);
        }

        [Fact]
        public void RequestsCorrectUrlWithApiOptions()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentsClient(connection);
            var expectedUrl = string.Format("repos/{0}/{1}/deployments", owner, name);

            var options = new ApiOptions
            {
                PageSize = 1,
                PageCount = 1,
                StartPage = 1
            };

            client.GetAll(owner, name, options);
            connection.Received(1)
                .GetAll<Deployment>(Arg.Is<Uri>(u => u.ToString() == expectedUrl), options);
        }
    }

    public class TheCreateMethod
    {
        private readonly NewDeployment newDeployment = new NewDeployment("aRef");

        [Fact]
        public async Task EnsuresNonNullArguments()
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Create(null, "name", newDeployment));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Create("owner", null, newDeployment));
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.Create("owner", "name", null));
        }

        [Fact]
        public async Task EnsuresNonEmptyArguments()
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.Create("", "name", newDeployment));
            await Assert.ThrowsAsync<ArgumentException>(() => client.Create("owner", "", newDeployment));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("\n\r")]
        public async Task EnsuresNonWhitespaceArguments(string whitespace)
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());

            await Assert.ThrowsAsync<ArgumentException>(() => client.Create(whitespace, "name", newDeployment));
            await Assert.ThrowsAsync<ArgumentException>(() => client.Create("owner", whitespace, newDeployment));
        }

        [Fact]
        public void PostsToDeploymentsUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentsClient(connection);
            var expectedUrl = "repos/owner/name/deployments";

            client.Create("owner", "name", newDeployment);

            connection.Received(1).Post<Deployment>(Arg.Is<Uri>(u => u.ToString() == expectedUrl),
                                                    Arg.Any<NewDeployment>());
        }

        [Fact]
        public void PassesNewDeploymentRequest()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new DeploymentsClient(connection);

            client.Create("owner", "name", newDeployment);

            connection.Received(1).Post<Deployment>(Arg.Any<Uri>(),
                                                    newDeployment);
        }
    }

    public class TheCtor
    {
        [Fact]
        public void EnsuresArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new DeploymentsClient(null));
        }

        [Fact]
        public void SetsStatusesClient()
        {
            var client = new DeploymentsClient(Substitute.For<IApiConnection>());
            Assert.NotNull(client.Status);
        }
    }
}