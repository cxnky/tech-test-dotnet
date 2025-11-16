The original payment service had a few issues which made it hard to test and maintain, so I refactored it to follow solid principles and make it much easier to test.

My main changes were: 
1. I created interfaces for dependencies - the old code was directly instantiating AccountDataStore and BackupAccountDataStore, and using ConfigurationManager directly which made it impossible to unit test properly as you cannot mock static classes
1. I extracted payment validation logic into separate classes - previously it was all in a big switch statement and each payment scheme has different rules so by extracting them into separate validators, each class has a single responsibility and can be tested independently. plus, adding a newpayment scheme in the future is now far easier
1. I created the configuration service, which wraps configurationmanager meaning that it can be mocked for testing
1. I centralised the logic for choosing between account data store and backup account data store, meaning that the logic is in one place and easier to test
1. I made the payment service take in interfaces in its primary constructor, meaning that all dependencies can be mocked for testing properly


As a result:
- the payment service is now much simpler and focused on orchestrating the payment process 
- each validator can be tested independently
- adding a new payment scheme requires zero changes to the payment service
- everything is testable with proper mocks 
- no code duplication 
- much easer to read, maintain and understand