<template>
	<QCardDialog
		:name="`${DialogType.AlertInfoDialog}-${alert.id}`"
		:type="{} as IAlert"
		width="500px"
		@closed="alertStore.removeAlert(alert.id)">
		<template #title>
			{{ alert.title }}
		</template>
		<template #default>
			<div>
				<pre style="white-space: break-spaces">{{ alert.text }}</pre>
				<QText>{{ $t('components.alert-dialog.request-data-sent') }}</QText>
				<pre
					v-if="alert.result"
					style="white-space: break-spaces">
				{{ alert.result }}
			</pre>
				<pre
					v-if="errors"
					style="white-space: break-spaces">
				{{ errors }}
			</pre>
			</div>
		</template>
		<template #actions="{ close }">
			<q-space />
			<!--	Close action	-->
			<BaseButton
				:label="$t('general.commands.close')"
				@click="close" />
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import type { IAlert } from '@interfaces';
import { DialogType } from '@enums';
import { useAlertStore } from '#imports';

const alertStore = useAlertStore();

const props = defineProps<{ alert: IAlert }>();

const errors = computed(() => {
	if (props.alert?.result?.errors) {
		return props.alert.result.errors;
	}
	return null;
});
</script>
