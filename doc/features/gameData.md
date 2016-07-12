# GameData
GameData is a storage system, following a key-value structure. It is used by SUGAR features and provides flexibility in providing custom game-specific storage solutions.

GameData provides storage for [Achievements](/articles/Achievements), [Skills](/articles/Skills), [Resources](/articles/Resources) and custom general data. 

## Features
* CRD GameData 
* Search GameData (by ID/Game/Actor/Skill)

## API
* Client
    * [GameDataClient](xref:PlayGen.SUGAR.Client.GroupMemberClient)
* Contracts
    * [GameDataRequest](xref:PlayGen.SUGAR.Contracts.GameDataRequest)
    * [GameDataResponse](xref:PlayGen.SUGAR.Contracts.GameDataResponse)
* WebAPI
    * [GameDataController](xref:PlayGen.SUGAR.WebAPI.Controllers.GameDataController)