import Log from 'consola';
import { acceptHMRUpdate, defineStore } from 'pinia';
import { get } from '@vueuse/core';
import { catchError, tap } from 'rxjs/operators';
import { DialogType } from '@enums';
import { plexAccountApi } from '@api';
import type { IError, PlexAccountDTO } from '@dto';
import type { IAccountDialog, ResultDTO } from '@interfaces';
import type { Observable } from 'rxjs';
import { cloneDeep, useAccountStore, useDialogStore } from '#imports';

interface IAccountDialogStore extends PlexAccountDTO {
	isAuthTokenMode: boolean;
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
		// Dialog properties
		isAuthTokenMode: false,
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

	const actions = {
		openDialog({ accountId }: IAccountDialog): void {
			state.isNewAccount = accountId === 0;
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
						dialogStore.openDialog(DialogType.AccountVerificationCodeDialog);
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
		validateVerificationToken(): Observable<ResultDTO<PlexAccountDTO>> {
			return plexAccountApi.validatePlexAccountEndpoint(get(getters.getAccountData)).pipe(
				tap(({ value, isSuccess }) => {
					if (isSuccess && value) {
						dialogStore.closeDialog(DialogType.AccountVerificationCodeDialog);
					} else {
						Log.error('Validate Error', value);
					}
				}),
			);
		},
		saveAccount() {
			state.savingLoading = true;
			if (state.isNewAccount) {
				return accountStore.createPlexAccount(get(getters.getAccountData)).pipe(
					tap(() => {
						state.savingLoading = false;
						dialogStore.closeDialog(DialogType.AccountDialog);
					}),
				);
			}
			return accountStore.updatePlexAccount(get(getters.getAccountData)).pipe(
				tap(() => {
					state.savingLoading = false;
					dialogStore.closeDialog(DialogType.AccountDialog);
				}),
			);
			// return iif(
			// 	() => state.isNewAccount,
			// 	accountStore.createPlexAccount(get(getters.getAccountData)),
			// 	accountStore.updatePlexAccount(get(getters.getAccountData)),
			// ).pipe(
			// 	tap(() => {
			// 		state.savingLoading = false;
			// 		dialogStore.closeDialog(DialogType.AccountDialog);
			// 	}),
			// );
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
