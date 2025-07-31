This commands are used to start two instances of the project on the same PC!!!!

First instance:
dotnet watch run --project TransferFileAPI --urls "http://localhost:5000"
dotnet watch run --project TransferFileUI --environment Development

Second instance:
dotnet watch run --project TransferFileAPI --urls "http://localhost:6000"
dotnet watch run --project TransferFileUI --environment Production --urls "http://localhost:6002;https://localhost:6003"
