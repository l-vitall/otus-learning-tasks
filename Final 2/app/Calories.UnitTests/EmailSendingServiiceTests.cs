// using Calories.Common.Models;
// using Calories.Common.Services;
// using Microsoft.Extensions.Options;
// using Xunit;
//
// namespace Calories.UnitTests
// {
//     class EmailSendingTestOptions : IOptions<EmailSendingSettings>
//     {
//         private readonly EmailSendingSettings _value = new EmailSendingSettings()
//         {
//             ApiKey = "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",
//             ConfirmationFrom = "vlukanin@gmail.com",
//             ConfirmationSubject = "Confirm your account",
//             ConfirmationContent = "Confirm your account by link: <a href='{callbackUrl}'>link</a>",
//             InvitationFrom = "vlukanin@gmail.com",
//             InvitationSubject = "Register",
//             InviteUserHtmlTemplate = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Simple registration at CaloriesApi</title><style> form {  width: 420px; } div {  margin-bottom: 20px; } label {  display: inline-block;  width: 240px;  text-align: right;  padding-right: 10px; } button, input {  float: right; } </style></head><body> <form action=\"{actionLink}\" method=\"post\"> <div>  <label for=\"firstname\">Firstname</label>  <input name=\"firstname\" id=\"say\" value=\"\"> </div> <div>  <label for=\"lastname\">Lastname</label>  <input name=\"lastname\" value=\"\"> </div> <div>  <label for=\"password\">Password</label>  <input name=\"password\" value=\"\"> </div> <div>  <label for=\"email\">Email</label>  <input name=\"email\" id=\"say\" value=\"{email}\" readonly> </div> <div>  <button>Register</button> </div> </form>  </body></html>"
//         };
//
//         public EmailSendingSettings Value => _value;
//     }
//     
//     public class EmailSendingServiiceTests
//     {
//         [Fact]
//         public void Manual_SendConfirmationEmail_ValidInput()
//         {
//             var sender = new EmailSendingService(new EmailSendingTestOptions());
//             var link =
//                 "https://localhost:5001/users/confirmEmail?email=jane@mailinator.com&code=CfDJ8MUS52skOe9LlV8%252fbsEPFiB2K%252fj6VoWMZQokqhWaYfiAze6XAsTx0GUwBPH2Oa7NyQTMoygIh%252fXgK87%252bHJdNfKiAonMJLQstyT9FstL4Hx4AoxmvjOhI9rIp9%252fUn77dGo1iy9iQq76gHbl7SCxjeof4ZHobL0hbf7SbthNbfi7qXghqhkVCPZCDjLGlfgrnI0i3hm3Ukx1OiDthCfXFZMOna6ImjwYMl2SRufklYpOU6LdInbR%252bnZgd5%252b4hpdJW1iQ%253d%253d";
//             
//             var result = sender.SendConfirmationEmailAsync("jane@mailinator.com", link).GetAwaiter().GetResult();
//             
//             Assert.True(result != null);
//         }
//         
//         [Fact]
//         public void Manual_SendInvitationEmail_ValidInput()
//         {
//             var sender = new EmailSendingService(new EmailSendingTestOptions());
//             var link =
//                 "https://localhost:5001/users/confirmEmail?email=jane@mailinator.com&code=CfDJ8MUS52skOe9LlV8%252fbsEPFiB2K%252fj6VoWMZQokqhWaYfiAze6XAsTx0GUwBPH2Oa7NyQTMoygIh%252fXgK87%252bHJdNfKiAonMJLQstyT9FstL4Hx4AoxmvjOhI9rIp9%252fUn77dGo1iy9iQq76gHbl7SCxjeof4ZHobL0hbf7SbthNbfi7qXghqhkVCPZCDjLGlfgrnI0i3hm3Ukx1OiDthCfXFZMOna6ImjwYMl2SRufklYpOU6LdInbR%252bnZgd5%252b4hpdJW1iQ%253d%253d";
//             
//             var result = sender.SendConfirmationEmailAsync("jane@mailinator.com", link).GetAwaiter().GetResult();
//             
//             Assert.True(result != null);
//         }
//     }
// }