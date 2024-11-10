import { acceptHMRUpdate, defineStore } from 'pinia';
import { type Observable, of, Subject } from 'rxjs';
import type { IAccountDialog, ISetupResult } from '@interfaces';
import { DialogType } from '@enums';
import type { CheckAllConnectionStatusUpdateDTO } from '@dto';

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
		openCheckServerConnectionsDialog(visible: boolean, data: CheckAllConnectionStatusUpdateDTO): void {
			state.dialogUpdate.next({ name: DialogType.CheckServerConnectionDialogName, state: visible, data });
		},
		openAccountDialog(visible: boolean, data: IAccountDialog): void {
			state.dialogUpdate.next({ name: DialogType.AccountDialog, state: visible, data });
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
