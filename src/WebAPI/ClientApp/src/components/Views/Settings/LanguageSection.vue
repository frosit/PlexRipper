<template>
	<QSection>
		<template #header>
			{{ $t('pages.settings.ui.language.header') }}
		</template>
		<HelpGroup class="q-mt-md">
			<HelpRow
				:label="$t('help.settings.ui.language.language-selection.label')"
				:title="$t('help.settings.ui.language.language-selection.title')"
				:text="$t('help.settings.ui.language.language-selection.text')">
				<LanguageSelect />
			</HelpRow>
		</HelpGroup>
	</QSection>
</template>
