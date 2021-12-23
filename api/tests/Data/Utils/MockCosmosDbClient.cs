using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Utils
{
    public class MockCosmosDbClient : ICosmosDbClient
    {
        private readonly Dictionary<string, Container> database;

        public MockCosmosDbClient()
        {
            this.database = new Dictionary<string, Container>();
        }

        public Container GetContainer(string containerName)
        {
            if (this.database.TryGetValue(containerName, out Container container))
            {
                return container;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void AddNewContainer(string containerName, Container data)
        {
            this.database.Add(containerName, data);
        }

        public void AddEmptyMemberContainer()
        {
            Container memberContainer = MockContainerProvider<Member>.CreateMockContainer(new List<Member>());
            this.AddNewContainer(ContainerConstants.MemberContainerName, memberContainer);
        }

        public void AddEmptyOrganizationContainer()
        {
            Container organizationContainer = MockContainerProvider<Organization>.CreateMockContainer(new List<Organization>());
            this.AddNewContainer(ContainerConstants.OrganizationContainerName, organizationContainer);
        }

        public void AddEmptyRaceContainer()
        {
            Container raceContainer = MockContainerProvider<Race>.CreateMockContainer(new List<Race>());
            this.AddNewContainer(ContainerConstants.RaceContainerName, raceContainer);
        }

        public void AddEmptyRaceResultContainer()
        {
            Container raceResultContainer = MockContainerProvider<RaceResult>.CreateMockContainer(new List<RaceResult>());
            this.AddNewContainer(ContainerConstants.RaceResultContainerName, raceResultContainer);
        }
    }
}
