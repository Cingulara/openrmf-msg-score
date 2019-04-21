# openrmf-msg-score
Messaging service to process new saves and updates of checklists for scoring. This listens for the specific subjects below and acts on them accordingly. It will save or update the scoring based on the Linq query in the classes
* openrmf.save.new
* openrmf.save.update
* openrmf.delete

## Running the NATS docker images
* docker run --rm --name nats-main -p 4222:4222 -p 6222:6222 -p 8222:8222 nats
* this is the default and lets you run a NATS server version 1.2.0 (as of 8/2018)
* just runs in memory and no streaming (that is separate)

## What is required
* .NET Core 2.x
* running `dotnet add package NATS.Client` to add the package
* dotnet restore to pull in all required libraries
* The C# NATS client library available at https://github.com/nats-io/csharp-nats

## Making your local Docker image
* make build
* make latest

## creating the database user
* ~/mongodb/bin/mongo 'mongodb://root:myp2ssw0rd@localhost'
* use admin
* db.createUser({ user: "openrmfscore" , pwd: "openrmf1234!", roles: ["readWriteAnyDatabase"]});
* use openrmfscore
* db.createCollection("Scores");

## connecting to the database collection straight
~/mongodb/bin/mongo 'mongodb://openrmfscore:openrmf1234!@localhost/openrmfscore?authSource=admin'

## List out the Scores you have inserted/updated
db.Scores.find();