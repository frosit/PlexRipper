<template>
	<QCardDialog
		:name="DialogType.AccountTokenValidateDialog"
		persistent
		width="700px"
		cy="auth-token-validation-dialog">
		<template #title>
			{{ $t('components.account-token-validate-dialog.title') }}
		</template>
		<template #top-row>
			<QSubHeader v-if="accountDialogStore.isValidated">
				{{ $t('components.account-token-validate-dialog.sub-header',
					{ authToken: accountDialogStore.authenticationToken })
				}}
			</QSubHeader>
		</template>
		<template #default>
			<div>
				<q-markup-table
					v-if="accountDialogStore.isValidated"
					wrap-cells>
					<tbody>
						<tr>
							<td>
								<QText :value="$t('components.account-token-validate-dialog.column.title')" />
							</td>
							<td>
								<QText :value="accountDialogStore.title" />
							</td>
						</tr>
						<tr>
							<td>
								<QText :value="$t('components.account-token-validate-dialog.column.username')" />
							</td>
							<td>
								<QText :value="accountDialogStore.username" />
							</td>
						</tr>
						<tr>
							<td>
								<QText :value="$t('components.account-token-validate-dialog.column.email')" />
							</td>
							<td>
								<QText :value="accountDialogStore.email" />
							</td>
						</tr>
					</tbody>
				</q-markup-table>

				<QAlert
					v-else
					cy="auth-token-validation-dialog-invalid-token-alert"
					type="error">
					{{ $t('components.account-token-validate-dialog.invalid-token') }}
				</QAlert>
			</div>
		</template>
		<template #actions="{ close }">
			<QRow justify="end">
				<!--	Hide	-->
				<QCol cols="auto">
					<HideButton
						cy="auth-token-validation-dialog-hide-button"
						@click="close" />
				</QCol>
			</QRow>
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import { DialogType } from '@enums';
import { useAccountDialogStore } from '#imports';

const accountDialogStore = useAccountDialogStore();
</script>
