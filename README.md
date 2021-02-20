![.NET Core Build and Test](https://github.com/Cingulara/openrmf-msg-score/workflows/.NET%20Core%20Build%20and%20Test/badge.svg)

# openrmf-msg-score
Messaging service to process new saves and updates of checklists for scoring. This listens for the specific subjects below and acts on them accordingly. It will save or update the scoring based on the Linq query in the classes.
* openrmf.checklist.save.new
* openrmf.checklist.save.update
* openrmf.checklist.delete
* openrmf.score.read
* openrmf.scores.system

## Running the NATS docker images
* docker run --rm --name nats-main -p 4222:4222 -p 6222:6222 -p 8222:8222 nats:2.1.2-linux
* this is the default and lets you run a NATS server version 2.x (as of 12/2019)
* just runs in memory and no streaming (that is separate)

## What is required
* .NET Core 3.1
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