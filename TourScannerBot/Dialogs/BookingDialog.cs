// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using TourScannerBot.ApiManager.CityApi;
using TourScannerBot.Models.CityApi;

namespace TourScannerBot.Dialogs
{
    public class BookingDialog : CancelAndHelpDialog
    {
        private readonly IConfiguration Configuration;
        private readonly HttpClient httpClient;

        public BookingDialog(IConfiguration configuration, HttpClient httpClient)
            : base(nameof(BookingDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DestinationStepAsync,
                DestinationConfirmStepAsync,
                TravelDateStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
            Configuration = configuration;
            this.httpClient = httpClient;
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.Destination == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Where would you like to travel to?") }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(bookingDetails.Destination, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> DestinationConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.Destination = (string)stepContext.Result;

            var cityApiManager = new CityApiManager(Configuration, httpClient);

            var countryResponse = await cityApiManager.ExecuteGetRequest(bookingDetails.Destination);
            var country = await countryResponse.Content.ReadAsAsync<CityModel>();

            var countryList = country.Data;

            if (countryList.Count > 1)
            { 
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                { Prompt = MessageFactory.Text("There are some variants, choose one :\n" + 
                this.CreateList(countryList)) }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(bookingDetails.Destination, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.Destination == null)
            {
                bookingDetails.Destination = (string)stepContext.Result;
            }
            else
            {
                bookingDetails.PossibleVariants = new List<string>();
                bookingDetails.PossibleVariants.Add((string)stepContext.Result);
            }

            if (bookingDetails.TravelDate == null || IsAmbiguous(bookingDetails.TravelDate))
            {
                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.TravelDate, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(bookingDetails.TravelDate, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.TravelDate = (string)stepContext.Result;
            var dateTime = DateTime.Parse(bookingDetails.TravelDate);
            bookingDetails.TravelDate = dateTime.ToLongDateString();

            var msg = $"Please confirm, Your tour trip is to : {bookingDetails.Destination} on : {bookingDetails.TravelDate}";



            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var bookingDetails = (BookingDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }

        private string CreateList(List<CityInfo> countryList)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < countryList.Count; i++)
            {
                sb.Append((i + 1) + ". " + countryList[i].Country);
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
