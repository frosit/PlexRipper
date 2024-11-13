import Log from 'consola';
import { acceptHMRUpdate, defineStore } from 'pinia';
import { get } from '@vueuse/core';
import { catchError, tap } from 'rxjs/operators';
import { DialogType } from '@enums';
import { plexAccountApi } from '@api';
import type { IError, PlexAccountDTO } from '@dto';
import type { IAccountDialog } from '@interfaces';
import { iif } from 'rxjs';
import { cloneDeep, useAccountStore, useDialogStore, useI18n } from '#imports';

interface IAccountDialogStore extends PlexAccountDTO {
	isNewAccount: boolean;
	isInputValid: boolean;
	showPassword: boolean;
	showAuthToken: boolean;
	deleteLoading: boolean;
	validateLoading: boolean;
	savingLoading: boolean;
	hasValidationErrors: boolean;
	validationErrors: IError[];
}

export const useAccountDialogStore = defineStore('AccountDialogStore', () => {
	const defaultState: IAccountDialogStore = {
		id: 0,
		isEnabled: true,
		isMain: true,
		username: '',
		password: '',
		displayName: '',
		clientId: '',
		verificationCode: '',
		uuid: '',
		hasPassword: false,
		validatedAt: '0001-01-01T00:00:00Z',
		is2Fa: false,
		title: '',
		plexId: 0,
		authenticationToken: '',
		email: '',
		plexServerAccess: [],
		plexLibraryAccess: [],
		isAuthTokenMode: false,
		// Dialog properties
		isNewAccount: false,
		isValidated: false,
		showAuthToken: false,
		showPassword: false,
		hasValidationErrors: false,
		deleteLoading: false,
		validateLoading: false,
		savingLoading: false,
		isInputValid: true,
		validationErrors: [],
	};

	const state = reactive<IAccountDialogStore>(cloneDeep(defaultState));
	const dialogStore = useDialogStore();
	const accountStore = useAccountStore();
	const { t } = useI18n();

	const actions = {
		openDialog({ accountId, isNewAccountValue }: IAccountDialog): void {
			state.isNewAccount = isNewAccountValue;
			if (!state.isNewAccount) {
				const account = accountStore.getAccount(accountId);
				if (account) {
					Object.assign(state, account);
				}
			}
		},
		closeDialog(): void {
			dialogStore.closeDialog(DialogType.AccountVerificationCodeDialog);
			dialogStore.closeDialog(DialogType.AccountConfirmationDialog);
			dialogStore.closeDialog(DialogType.AccountDialog);
			actions.reset();
		},
		validatePlexAccount() {
			state.validateLoading = true;

			return plexAccountApi.validatePlexAccountEndpoint(get(getters.getAccountData)).pipe(
				tap(({ value, isSuccess }) => {
					if (!isSuccess) {
						return;
					}
					const account = isSuccess ? value : null;

					state.hasValidationErrors = false;
					state.validateLoading = false;

					Object.assign(state, account);

					if (account?.isValidated && account?.isAuthTokenMode) {
						Log.info('Account is validated and was added by token');
						dialogStore.openDialog(DialogType.AccountTokenValidateDialog);
						return;
					}

					// Account has no 2FA and was valid
					if (account?.isValidated && !account?.is2Fa) {
						Log.info('Account has no 2FA and was valid');
						return;
					}

					// Account has no 2FA and was invalid
					if (!account?.isValidated && !account?.is2Fa) {
						Log.info('Account has no 2FA and was invalid');
						return;
					}

					// Account has 2FA
					if (!account?.isValidated && account?.is2Fa) {
						Log.info('Account has 2FA enabled');
						return;
					}

					if (!account?.isValidated && account?.is2Fa) {
						Log.info('Account was valid and has 2FA enabled, this makes no sense and sounds like a bug');
					}
				}),
				catchError((val) => {
					state.isValidated = false;
					state.hasValidationErrors = true;
					state.validateLoading = false;
					dialogStore.openDialog(DialogType.AccountTokenValidateDialog);
					return val;
				}),
			);
		},
		validateVerificationToken() {
			return plexAccountApi.validatePlexAccountEndpoint(get(getters.getAccountData)).pipe(
				tap(({ value, isSuccess }) => {
					if (isSuccess && value) {
						dialogStore.closeDialog(DialogType.AccountVerificationCodeDialog);
					} else {
						Log.error('Validate Error', value);
					}
				}),
				catchError((val) => {
					Log.error('Validate Error', val);
					return val;
				}),
			);
		},
		saveAccount() {
			state.savingLoading = true;
			return iif(
				() => state.isNewAccount,
				accountStore.createPlexAccount(get(getters.getAccountData)),
				accountStore.updatePlexAccount(get(getters.getAccountData)),
			).pipe(
				tap(() => {
					state.savingLoading = false;
					dialogStore.closeDialog(DialogType.AccountDialog);
				}),
			);
		},
		deleteAccount() {
			state.deleteLoading = true;
			return accountStore.deleteAccount(state.id).pipe(tap(() => dialogStore.closeDialog(DialogType.AccountDialog)));
		},
		reset() {
			Object.assign(state, defaultState);
		},
	};
	const getters = {
		getDialogHeader: computed(() => {
			const displayName = state.displayName;
			let title: string;
			if (state.isNewAccount) {
				title = t('components.account-dialog.add-account-title', {
					name: state.displayName,
				});
			} else {
				title = t('components.account-dialog.edit-account-title', {
					name: state.displayName,
				});
			}
			// Remove the colon if the display name is empty
			return displayName ? title : title.replace(':', '');
		}),
		hasCredentialsChanged: computed(() => {
			if (!state.isNewAccount) {
				const originalPlexAccount = accountStore.getAccount(state.id);
				if (!originalPlexAccount) {
					return false;
				}

				return originalPlexAccount.username !== state.username || originalPlexAccount.password !== state.password;
			}
			return false;
		}),
		getAccountData: computed((): PlexAccountDTO => {
			return {
				id: state.id,
				isValidated: state.isValidated,
				password: state.password,
				username: state.username,
				uuid: state.uuid,
				validatedAt: state.validatedAt,
				verificationCode: state.verificationCode,
				authenticationToken: state.authenticationToken,
				clientId: state.clientId,
				displayName: state.displayName,
				email: state.email,
				hasPassword: state.hasPassword,
				is2Fa: state.is2Fa,
				isEnabled: state.isEnabled,
				isMain: state.isMain,
				isAuthTokenMode: state.isAuthTokenMode,
				plexId: state.plexId,
				plexLibraryAccess: state.plexLibraryAccess,
				plexServerAccess: state.plexServerAccess,
				title: state.title,
			};
		}),
		validationStyle: computed(
			(): {
				color: 'default' | 'positive' | 'warning' | 'negative';
				icon: string;
				text: string;
			} => {
				if (state.hasValidationErrors) {
					return {
						color: 'negative',
						icon: 'mdi-alert-circle-outline',
						text: t('general.commands.validate'),
					};
				}
				if (state.isValidated && !state.hasValidationErrors) {
					return {
						color: 'positive',
						icon: 'mdi-check-bold',
						text: '',
					};
				}
				return {
					color: 'default',
					icon: 'mdi-text-box-search-outline',
					text: t('general.commands.validate'),
				};
			},
		),
	};
	return {
		...toRefs(state),
		...actions,
		...getters,
	};
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useAccountDialogStore, import.meta.hot));
}
