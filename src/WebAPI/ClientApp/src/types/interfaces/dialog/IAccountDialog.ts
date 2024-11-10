import type { PlexAccountDTO } from '@dto';

export interface IAccountDialog {
	isNewAccountValue: boolean;
	account: PlexAccountDTO | null;
}
