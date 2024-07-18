
# TIPOS - IGT API
API for obtaining a deposit code via the IGT terminal

## Version: 1.0

## Terms
* __eTip API__ - Application Programming Interface for IGT terminals
* __eTip API server__ - Server, which allows to communicate with IGT terminals via defined web api interface 
* __SharedSecret__ - Configurable value on API server 

## Communication framework
IGT API is based on REST architecture. It is a way to easily create, read, edit or delete information from a server using simple HTTP call.

### Request
Url = {scope}/v{version}/{adapter}/{method}  
Example:
> /api/igt/v1/isalive

### Response

## HTTP status codes
* 200 - OK
* 400 - Bad request
* 401 - Unauthorized
* 500 - Internal server error

### Communication overview
![C4 diagram](/IGT-deposit-code.drawio.png)

### Authorization
REST API used to HMAC authentication access
[GitHub](https://github.com/cuongle/Hmac.WebApi
)

__Client auth - create request with timestamp  and authorization header value  based  on request content, timestamp and credentials__

Example:
> curl -X POST \ https://tst-services.etipos.sk/api/igt/v1/transaction/depositcode \
> -H 'accept: application/json' \
> -H 'content-type: application/json' \
> -H 'timestamp: 2024-07-17 05:55:06Z' \
> -H 'authentication: Igt:MalY/Na4Ym/7JcI4IQextnaEgJT799tTDC6x45/v9Bs=' \
> -d '{"Amount": 20}'

### Message signing - authentication
1. Create request _timestamp_ in UTC format and add this to request header
>Example: request header = 'timestamp: 2024-07-17 05:55:06Z'

2. Create request _message_ by joining parts _timestamp_, _request-uri_ and _request-content_ separated by a character |
>Example: message = "2024-07-17 05:55:06Z|/api/igt/v1/transaction/depositcode|{"Amount": 20}"

3. Create request sign as HMAC hash from _message_ and _sharedSecret_ as key
>Example: request message = "Igt:MalY/Na4Ym/7JcI4IQextnaEgJT799tTDC6x45/v9Bs=:
```csharp
private static string ComputeHash(string sharedSecret, string message)
{
    var key = Encoding.UTF8.GetBytes(sharedSecret);
    string hashString;
    using (var hmac = new HMACSHA256(key))
    {
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        hashString = Convert.ToBase64String(hash);
    }
    return hashString; 
}
```
4. Add _sign_ as _authentication_ header value
>Example: request header = 'authentication: Igt:MalY/Na4Ym/7JcI4IQextnaEgJT799tTDC6x45/v9Bs='

### Authentication note
Each request must have a unique authentication sign, as it also includes a timestamp. A repeated request with the same sign will be rejected as unauthorized.

### Integration environment

**Restricted access**  
Please contact us
* To whitelist your IP address to gain access to integration environment
* To provide URL of integration environment
* To provide sharedSecret for message signing 

## FAQ


### EBet IGT API Methods

## IsAlive 
* HTTP method: GET
* URI: /api/igt/v1/isalive

### Response 

|Property |Type|Description |
|-|:-|:-|
|time|DateTime|Current UTC  
|systemState|int|Current API state

#### SystemState values
* 1 = API ready
* 5 = API not ready

#### Example: 
```json
{
  "time":"2024-07-17T05:34:16.483Z",
  "systemState":1
}
```

## DepositCode 
* HTTP method: POST
* URI: /api/igt/v1/transaction/depositcode

### Request 

|Property |Type|Description |
|:-|:-|:-|
|amount|decimal|Deposit coupon amount

#### Example 
```json
{
    "amount":20
}
```

### Response
|Property |Type|Description |
|-|:-|:-|
|code|string|generated deposit code
|error|int|operation result error (see ref)

#### Example 
```json
{
    "code":"1234-5678-1234-5678",
    "error":0
}
```

## Cancel 
* HTTP method: PUT
* URI: /api/igt/v1/transaction/cancel

## Note 
The code can be canceled within 3 minutes of creation. After this time, the cancellation request will be rejected.

### Request 

|Property |Type|Description |
|:-|:-|:-|
|code|string|Deposit coupon code

#### Example 
```json 
{
    "code":"1234-5678-1234-5678"
}
```

### Response
|Property |Type|Description |
|-|:-|:-|
|error|int|operation result error (see ref)

#### Example 
```json
{   
    "error":0
}
```

## Commit 
* HTTP method: PUT
* URI: /api/igt/v1/transaction/commit

### Request 

|Property |Type|Description |
|:-|:-|:-|
|code|string|Deposit coupon code

#### Example 
```json 
{
    "code":"1234-5678-1234-5678"
}
```

### Response
|Property |Type|Description |
|-|:-|:-|
|error|int|operation result error (see ref)

#### Example 
```json
{    
    "error":0
}
```
#### Error codes
|Number|Name|Description|
|-|-|-|
|0|Success|
|1|CommonError|Other than specified error below
|2|InvalidAmount|Amount is not valid decimal number
|3|InvalidCode|Deposit Code is invalid (empty atc...)
|4|NotFound|Requested dposit Code not found (for Cancel, Commit..)
|5|InvalidState|Code hasn't valid state for requested operation (Example: Commit canceled code etc.)
|6|CancelPeriodExpired|| 
