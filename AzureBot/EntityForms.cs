﻿namespace AzureBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureBot.Azure.Management.ResourceManagement;
    using FormTemplates;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.FormFlow.Advanced;

    public class EntityForms
    {
        public static IForm<SubscriptionFormState> BuildSubscriptionForm()
        {
            return new FormBuilder<SubscriptionFormState>()
                .Field(new FieldReflector<SubscriptionFormState>(nameof(SubscriptionFormState.SubscriptionId))
                .SetType(null)
                .SetPrompt(new PromptAttribute("Please select the subscription you want to work with: {||}"))
                .SetDefine(async (state, field) =>
               {
                   var subscriptions = await new AzureRepository().ListSubscriptionsAsync();
                   foreach (var sub in subscriptions)
                   {
                       field.AddDescription(sub.SubscriptionId, sub.DisplayName)
                           .AddTerms(sub.SubscriptionId, sub.DisplayName);
                   }

                   return true;
               }))
               .Build();
        }

        public static IForm<VirtualMachineFormState> BuildVirtualMachinesForm()
        {
            return new FormBuilder<VirtualMachineFormState>()
                .Field(nameof(VirtualMachineFormState.Operation))
                .Field(new FieldReflector<VirtualMachineFormState>(nameof(VirtualMachineFormState.VirtualMachine))
                .SetType(null)
                .SetPrompt(new PromptAttribute("Please select the virtual machine you want to {Operation}: {||}"))
                .SetDefine((state, field) =>
                {
                    foreach (var vm in state.AvailableVMs)
                    {
                        field
                            .AddDescription(vm, vm)
                            .AddTerms(vm, vm);
                    }

                    return Task.FromResult(true);
                }))
               .Confirm("Would you like to {Operation} virtual machine '{VirtualMachine}'?")
               .Build();
        }

        internal static IForm<SubscriptionFormState> BuildSubscriptionForm(List<string> list)
        {
            throw new NotImplementedException();
        }

        public static IForm<RunBookFormState> BuildRunBookForm()
        {
            return new FormBuilder<RunBookFormState>()
                .Field(new FieldReflector<RunBookFormState>(nameof(RunBookFormState.AutomationAccountName))
                    .SetType(null)
                    .SetPrompt(new PromptAttribute("Please select the automation account you want to use: {||}"))
                    .SetDefine((state, field) =>
                    {
                        foreach (var account in state.AvailableAutomationAccounts)
                        {
                            field
                                .AddDescription(account.AutomationAccountName, account.AutomationAccountName)
                                .AddTerms(account.AutomationAccountName, account.AutomationAccountName);
                        }

                        return Task.FromResult(true);
                    }))
                .Field(new FieldReflector<RunBookFormState>(nameof(RunBookFormState.RunBookName))
                    .SetType(null)
                    .SetPrompt(new PromptAttribute("Please select the runbook you want to run: {||}"))
                    .SetActive(state=> !string.IsNullOrWhiteSpace(state.AutomationAccountName))
                    .SetDefine((state, field) =>
                    {
                        if (string.IsNullOrWhiteSpace(state.AutomationAccountName))
                        {
                            return Task.FromResult(false);
                        }

                        foreach (var runBook in state.AvailableAutomationAccounts.Single(
                            a => a.AutomationAccountName == state.AutomationAccountName).RunBooks)
                        {
                            field
                                .AddDescription(runBook.RunBookName, runBook.RunBookName)
                                .AddTerms(runBook.RunBookName, runBook.RunBookName);
                        }

                        return Task.FromResult(true);
                    }))
               .Confirm("Would you like to run runbook '{RunBookName}'?")
               .Build();
        }
    }
}