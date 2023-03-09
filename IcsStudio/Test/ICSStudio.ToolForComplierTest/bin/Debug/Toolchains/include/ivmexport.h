#ifndef RUNTIME_ELF_IVM_EXPORT_H
#define RUNTIME_ELF_IVM_EXPORT_H

struct runtime_symbol {
  const char* name;
  unsigned long value;
};

#define RUNTIME_SYMBOL_PREFIX ""

#define __EXPORT_SYMBOL(sym)                                                                                      \
  static const char __rstrtab_##sym[] __attribute__((section("__rsymtab_strings"))) = RUNTIME_SYMBOL_PREFIX #sym; \
  static const struct runtime_symbol __rsymtab_##sym                                                              \
      __attribute__((section("__rsymtab+" #sym), used)) = {__rstrtab_##sym, (unsigned long)&sym}

#define EXPORT_SYMBOL(sym) __EXPORT_SYMBOL(sym)

#endif
