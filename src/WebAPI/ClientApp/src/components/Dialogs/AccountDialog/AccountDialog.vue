<template>
	<QCardDialog
		max-width="900px"
		:name="DialogType.AccountDialog"
		persistent
		:type="{} as IAccountDialog"
		cy="account-dialog-form"
		@opened="accountDialogStore.openDialog"
		@closed="accountDialogStore.closeDialog">
		<!-- Dialog Header -->
		<template #title>
			<QRow>
				<QCol>
					<QText
						size="h5"
						bold="bold">
						{{ accountDialogStore.getDialogHeader }}
					</QText>
				</QCol>
				<!-- Auth Token Mode -->
				<QCol cols="auto">
					<IconButton
						icon="mdi-cloud-key-outline"
						cy="account-dialog-auth-token-mode-button"
						@click="accountDialogStore.isAuthTokenMode = !accountDialogStore.isAuthTokenMode" />
				</QCol>
			</QRow>
		</template>
		<template #default>
			<div>
				<AccountForm />
				<Print>{{ accountDialogStore.$state }}</Print>
			</div>
		</template>
		<!-- Dialog Actions	-->
		<template #actions="{ close }">
			<QRow
				justify="between"
				gutter="md">
				<!-- Delete account -->
				<QCol v-if="!accountDialogStore.isNewAccount">
					<DeleteButton
						class="mx-2"
						block
						cy="account-dialog-delete-button"
						@click="dialogStore.openDialog(DialogType.AccountConfirmationDialog)" />
				</QCol>
				<!-- Cancel button -->
				<QCol>
					<CancelButton
						class="mx-2"
						block
						cy="account-dialog-cancel-button"
						@click="close" />
				</QCol>
				<!-- Reset Form -->
				<QCol>
					<ResetButton
						class="mx-2"
						block
						cy="account-dialog-reset-button"
						@click="accountDialogStore.reset" />
				</QCol>
				<!-- Validation button -->
				<QCol>
					<AccountValidationButton
						:color="accountDialogStore.validationStyle.color"
						:disabled="accountDialogStore.validateLoading"
						:icon="accountDialogStore.validationStyle.icon"
						:label="accountDialogStore.validationStyle.text"
						:loading="accountDialogStore.validateLoading"
						block
						cy="account-dialog-validate-button"
						class="q-mx-md"
						@click="validatePlexAccount" />
				</QCol>
				<!-- Save account -->
				<QCol>
					<SaveButton
						:label="accountDialogStore.isNewAccount ? $t('general.commands.save') : $t('general.commands.update')"
						:cy="`account-dialog-${accountDialogStore.isNewAccount ? 'save' : 'update'}-button`"
						block
						:loading="accountDialogStore.savingLoading"
						class="q-mx-md"
						@click="saveAccount" />
				</QCol>
			</QRow>
		</template>
	</QCardDialog>

	<!--	Account Verification Code Dialog	-->
	<AccountVerificationCodeDialog
		:account="accountDialogStore.$state" />

	<AccountTokenValidateDialog :account="accountDialogStore.$state" />

	<!--	Delete Confirmation Dialog	-->
	<ConfirmationDialog
		class="q-mr-md"
		:confirm-loading="accountDialogStore.deleteLoading"
		:name="DialogType.AccountConfirmationDialog"
		:title="$t('confirmation.delete-account.title')"
		:text="$t('confirmation.delete-account.text')"
		:warning="$t('confirmation.delete-account.warning')"
		@confirm="deleteAccount" />
</template>

<script setup lang="ts">
import { useSubscription } from '@vueuse/rxjs';
import type { IAccountDialog } from '@interfaces';
import { DialogType } from '@enums';
import { useAccountDialogStore } from '@store';
import { useDialogStore } from '#imports';

const accountDialogStore = useAccountDialogStore();
const dialogStore = useDialogStore();

function validatePlexAccount() {
	useSubscription(accountDialogStore.validatePlexAccount().subscribe());
}

function deleteAccount() {
	useSubscription(accountDialogStore.deleteAccount().subscribe());
}

function saveAccount() {
	useSubscription(accountDialogStore.saveAccount().subscribe());
}
</script>
