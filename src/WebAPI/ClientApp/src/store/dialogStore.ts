import { acceptHMRUpdate, defineStore } from 'pinia';
import { type Observable, of, Subject } from 'rxjs';
import { DialogType } from '@enums';
import type { IAccountDialog, IAlert, IHelp, ISetupResult } from '@interfaces';
import type { CheckAllConnectionStatusUpdateDTO, DownloadMediaDTO, FolderPathDTO } from '@dto';

interface DialogState {
	name: string;
	state: boolean;
	data: unknown;
}

export const useDialogStore = defineStore('DialogStore', () => {
	const state = reactive<{ dialogUpdate: Subject<DialogState> }>({
		dialogUpdate: new Subject<DialogState>(),
	});
	const actions = {
		setup(): Observable<ISetupResult> {
			return of({ name: useDialogStore.name, isSuccess: true });
		},
		closeDialog(name: DialogType): void {
			state.dialogUpdate.next({ name, state: false, data: {} as unknown });
		},
		openDialog(name: DialogType): void {
			state.dialogUpdate.next({ name, state: true, data: {} as unknown });
		},
		openCheckServerConnectionsDialog(data: CheckAllConnectionStatusUpdateDTO): void {
			state.dialogUpdate.next({ name: DialogType.CheckServerConnectionDialogName, state: true, data });
		},
		openAccountDialog(data: IAccountDialog): void {
			state.dialogUpdate.next({ name: DialogType.AccountDialog, state: true, data });
		},
		openDirectoryBrowserDialog(data: FolderPathDTO): void {
			state.dialogUpdate.next({ name: DialogType.DirectoryBrowserDialog, state: true, data });
		},
		openServerSettingsDialog(plexServerId: number): void {
			state.dialogUpdate.next({ name: DialogType.ServerSettingsDialog, state: true, data: plexServerId });
		},
		openDownloadTaskDetailsDialog(downloadTaskId: string): void {
			state.dialogUpdate.next({ name: DialogType.DownloadDetailsDialog, state: true, data: downloadTaskId });
		},
		openMediaConfirmationDownloadDialog(data: DownloadMediaDTO[]): void {
			state.dialogUpdate.next({ name: DialogType.MediaDownloadConfirmationDialog, state: true, data });
		},
		openAddConnectionDialog(plexServerId: number): void {
			state.dialogUpdate.next({ name: DialogType.AddConnectionDialog, state: true, data: plexServerId });
		},
		openHelpInfoDialog(data: IHelp): void {
			state.dialogUpdate.next({ name: DialogType.HelpInfoDialog, state: true, data });
		},
		openAlertInfoDialog(alert: IAlert): void {
			state.dialogUpdate.next({ name: `${DialogType.AlertInfoDialog}-${alert.id}`, state: true, data: alert });
		},
	};
	const getters = {
		getDialogState: (): Observable<DialogState> => {
			return state.dialogUpdate.asObservable();
		},
	};
	return {
		...toRefs(state),
		...actions,
		...getters,
	};
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useDialogStore, import.meta.hot));
}
