#ifndef IVM_BYTECODE_IMPL_H
#define IVM_BYTECODE_IMPL_H
#include <stdatomic.h>

typedef unsigned char u8;
typedef signed char s8;
typedef short int s16;
typedef int s32;
typedef unsigned int u32;
typedef long long s64;

typedef s32 int32_t;
typedef u32 uint32_t;
typedef s16 int16_t;
typedef s8 int8_t;
typedef s64 int64_t;
typedef u8 uint8_t;

union Operand {
  s32 ivalue;
  s64 lvalue;
  float fvalue;
  double dvalue;
  void* pvalue;
};

void ivm_builtin_stack_check();
s64 ivm_double_to_long(double value);
s64 ivm_float_to_long(float value);
s64 ivm_builtin_lmod(s64 lhs, s64 rhs);
s32 ivm_builtin_imod(s32 lhs, s32 rhs);
s64 ivm_builtin_ldiv(s64 lhs, s64 rhs);
s32 ivm_builtin_idiv(s32 lhs, s32 rhs);
float ivm_builtin_floorf(float value);
double ivm_builtin_floor(double value);

float ivm_builtin_ceilf(float value);
double ivm_builtin_ceil(double value);
float ivm_builtin_fmodf(float lhs, float rhs);
float ivm_builtin_fmod(double lhs, double rhs);

static float ivm_builtin_fabsf(float value) { return value > 0 ? value : -value; }
static double ivm_builtin_fabs(double value) { return value > 0 ? value : -value; }

s32 ivm_builtin_throw(s32 type, s32 code);
s32 ivm_builtin_push(u32 type, const char* name, u32 line);
s32 ivm_builtin_pop();

float dutch_round_float(float value) {
  float val1;
  float decimals;

  if (value >= 0.0) {
    decimals = value - ivm_builtin_floorf(value);
    if (decimals > 0.5) {
      val1 = ivm_builtin_ceilf(value);
    } else if (decimals < 0.5) {
      val1 = ivm_builtin_floorf(value);
    } else {
      s32 is_even = ((ivm_float_to_long(value - decimals) & 0x01) == 0);
      if (is_even) {
        val1 = ivm_builtin_floorf(value);
      } else {
        val1 = ivm_builtin_ceilf(value);
      }
    }
  } else {
    decimals = ivm_builtin_fabsf(value + ivm_builtin_floorf(ivm_builtin_fabsf(value)));
    if (decimals > 0.5) {
      val1 = ivm_builtin_floorf(value);
    } else if (decimals < 0.5) {
      val1 = ivm_builtin_ceilf(value);
    } else {
      s32 is_even = ((ivm_float_to_long(value + decimals) & 0x01) == 0);
      if (is_even) {
        val1 = ivm_builtin_ceilf(value);
      } else {
        val1 = ivm_builtin_floorf(value);
      }
    }
  }

  return val1;
}

static inline s32 dutch_round_float_int32(float value) { return dutch_round_float(value); }
static inline s64 dutch_round_float_int64(float value) { return dutch_round_float(value); }

double dutch_round_double(double value) {
  double val1;
  double decimals;

  if (value >= 0.0) {
    decimals = value - ivm_builtin_floor(value);
    if (decimals > 0.5) {
      val1 = ivm_builtin_ceil(value);
    } else if (decimals < 0.5) {
      val1 = ivm_builtin_floor(value);
    } else {
      s32 is_even = ((ivm_double_to_long(value - decimals) & 0x01) == 0);
      if (is_even) {
        val1 = ivm_builtin_floor(value);
      } else {
        val1 = ivm_builtin_ceil(value);
      }
    }
  } else {
    decimals = ivm_builtin_fabs(value + ivm_builtin_floor(ivm_builtin_fabs(value)));
    if (decimals > 0.5) {
      val1 = ivm_builtin_floor(value);
    } else if (decimals < 0.5) {
      val1 = ivm_builtin_ceil(value);
    } else {
      s32 is_even = ((ivm_double_to_long(value + decimals) & 0x01) == 0);
      if (is_even) {
        val1 = ivm_builtin_ceil(value);
      } else {
        val1 = ivm_builtin_floor(value);
      }
    }
  }

  return val1;
}

static inline s32 dutch_round_double_int32(double value) { return dutch_round_double(value); }
static inline s64 dutch_round_double_int64(double value) { return dutch_round_double(value); }

static inline s32 ivm_load_int8_bit(s8* ptr, int offset) {
  s8 value = __atomic_load_n(ptr, __ATOMIC_RELAXED);
  return (value >> offset) & 0x01;
}

static inline s32 ivm_load_int16_bit(s16* ptr, int offset) {
  s16 value = __atomic_load_n(ptr, __ATOMIC_RELAXED);
  return (value >> offset) & 0x01;
}

static inline s32 ivm_load_int32_bit(s32* ptr, int offset) {
  s32 value = __atomic_load_n(ptr, __ATOMIC_RELAXED);
  return (value >> offset) & 0x01;
}

static inline s64 ivm_load_int64_bit(s64* ptr, int offset) {
  s64 value = __atomic_load_n(ptr, __ATOMIC_RELAXED);
  return (value >> offset) & 0x01;
}

static inline s8 ivm_load_int8(s8* ptr) { return __atomic_load_n(ptr, __ATOMIC_RELAXED); }
static inline s16 ivm_load_int16(s16* ptr) { return __atomic_load_n(ptr, __ATOMIC_RELAXED); }
static inline s32 ivm_load_int32(s32* ptr) { return __atomic_load_n(ptr, __ATOMIC_RELAXED); }
static inline s64 ivm_load_int64(s64* ptr) { return __atomic_load_n(ptr, __ATOMIC_RELAXED); }
static inline float ivm_load_float(float* ptr) {
  float res;
  __atomic_load((s32*)ptr, (s32*)&res, __ATOMIC_RELAXED);
  return res;
}

static inline double ivm_load_double(double* ptr) {
  double res;
  __atomic_load((s64*)ptr, (s64*)&res, __ATOMIC_RELAXED);
  return res;
}

static inline void ivm_store_int8_bit(s8* ptr, int offset, int value) {
  if (value == 0) {
    __atomic_fetch_and(ptr, ~(1 << offset), memory_order_relaxed);
  } else {
    __atomic_fetch_or(ptr, 1 << offset, memory_order_relaxed);
  }

  return;
  s8 tmp = *ptr;
  s8 desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  while (__atomic_compare_exchange_n(ptr, &tmp, desired, 1, __ATOMIC_RELAXED, __ATOMIC_RELAXED)) {
    desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  }
}

static inline void ivm_store_int16_bit(s16* ptr, int offset, int value) {
  if (value == 0) {
    __atomic_fetch_and(ptr, ~(1 << offset), memory_order_relaxed);
  } else {
    __atomic_fetch_or(ptr, 1 << offset, memory_order_relaxed);
  }

  return;
  s16 tmp = *ptr;
  s16 desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  while (__atomic_compare_exchange_n(ptr, &tmp, desired, 1, __ATOMIC_RELAXED, __ATOMIC_RELAXED)) {
    desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  }
}

static inline void ivm_store_int32_bit(s32* ptr, int offset, int value) {
  if (value == 0) {
    __atomic_fetch_and(ptr, ~(1 << offset), memory_order_relaxed);
  } else {
    __atomic_fetch_or(ptr, 1 << offset, memory_order_relaxed);
  }
  return;
  s32 tmp = *ptr;
  s32 desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  while (__atomic_compare_exchange_n(ptr, &tmp, desired, 1, __ATOMIC_RELAXED, __ATOMIC_RELAXED)) {
    desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  }
}

static inline void ivm_store_int64_bit(s64* ptr, int offset, int value) {
  if (value == 0) {
    __atomic_fetch_and(ptr, ~(1ULL << offset), memory_order_relaxed);
  } else {
    __atomic_fetch_or(ptr, 1ULL << offset, memory_order_relaxed);
  }
  return;
  s64 tmp = *ptr;
  s64 desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  while (__atomic_compare_exchange_n(ptr, &tmp, desired, 1, __ATOMIC_RELAXED, __ATOMIC_RELAXED)) {
    desired = ((tmp) & (~(0x01 << offset))) | ((value & 0x01) << offset);
  }
}

static inline void ivm_store_int8(s8* ptr, s8 value) { __atomic_store_n(ptr, value, __ATOMIC_RELAXED); }
static inline void ivm_store_int16(s16* ptr, s16 value) { __atomic_store_n(ptr, value, __ATOMIC_RELAXED); }
static inline void ivm_store_int32(s32* ptr, s32 value) { __atomic_store_n(ptr, value, __ATOMIC_RELAXED); }
static inline void ivm_store_int64(s64* ptr, s64 value) { __atomic_store_n(ptr, value, __ATOMIC_RELAXED); }
static inline void ivm_store_float(float* ptr, float value) {
  __atomic_store((s32*)ptr, (s32*)&value, __ATOMIC_RELAXED);
}
static inline void ivm_store_double(double* ptr, double value) {
  __atomic_store((s64*)ptr, (s64*)&value, __ATOMIC_RELAXED);
}

static inline char* ivm_pbinc(char* ptr, int offset) { return ptr + offset; }
static inline char* ivm_psinc(char* ptr, int offset) { return ptr + offset; }
static inline char* ivm_padd(char* ptr, int offset) { return ptr + offset; }

static inline s32 ivm_iadd(s32 lhs, s32 rhs) { return lhs + rhs; }
static inline s32 ivm_isub(s32 lhs, s32 rhs) { return lhs - rhs; }
static inline s32 ivm_imul(s32 lhs, s32 rhs) { return lhs * rhs; }
static inline s32 ivm_idiv(s32 lhs, s32 rhs) { return ivm_builtin_idiv(lhs, rhs); }
static s32 ivm_imod(s32 lhs, s32 rhs) {
  if (rhs == 0) rhs = lhs + 1;
  return ivm_builtin_imod(lhs, rhs);
}

static inline s32 ivm_ior(s32 lhs, s32 rhs) { return lhs | rhs; }
static inline s32 ivm_ixor(s32 lhs, s32 rhs) { return lhs ^ rhs; }
static inline s32 ivm_iand(s32 lhs, s32 rhs) { return lhs & rhs; }

static inline s64 ivm_ladd(s64 lhs, s64 rhs) { return lhs + rhs; }
static inline s64 ivm_lsub(s64 lhs, s64 rhs) { return lhs - rhs; }
static inline s64 ivm_lmul(s64 lhs, s64 rhs) { return lhs * rhs; }
static inline s64 ivm_ldiv(s64 lhs, s64 rhs) { return ivm_builtin_ldiv(lhs, rhs); }

static s64 ivm_lmod(s64 lhs, s64 rhs) {
  if (rhs == 0) rhs = lhs + 1;
  return ivm_builtin_lmod(lhs, rhs);
}

static inline s64 ivm_lor(s64 lhs, s64 rhs) { return lhs | rhs; }
static inline s64 ivm_lxor(s64 lhs, s64 rhs) { return lhs ^ rhs; }
static inline s64 ivm_land(s64 lhs, s64 rhs) { return lhs & rhs; }

static inline float ivm_fadd(float lhs, float rhs) { return lhs + rhs; }
static inline float ivm_fsub(float lhs, float rhs) { return lhs - rhs; }
static inline float ivm_fmul(float lhs, float rhs) { return lhs * rhs; }
static inline float ivm_fdiv(float lhs, float rhs) { return lhs / rhs; }
static inline float ivm_fmod(float lhs, float rhs) {
  if (rhs == 0) rhs = lhs + 1;
  return ivm_builtin_fmodf(lhs, rhs);
}

static inline double ivm_dadd(double lhs, double rhs) { return lhs + rhs; }
static inline double ivm_dsub(double lhs, double rhs) { return lhs - rhs; }
static inline double ivm_dmul(double lhs, double rhs) { return lhs * rhs; }
static inline double ivm_ddiv(double lhs, double rhs) { return lhs / rhs; }
static inline double ivm_dmod(double lhs, double rhs) {
  if (rhs == 0) rhs = lhs + 1;
  return ivm_builtin_fmod(lhs, rhs);
}

static inline s32 ivm_icmp(s32 lhs, s32 rhs) {
  s32 cmp = lhs - rhs;
  if (cmp > 0) return 1;
  if (cmp == 0) return 0;
  return -1;
}

static inline s32 ivm_lcmp(s64 lhs, s64 rhs) {
  s64 cmp = lhs - rhs;
  if (cmp > 0) return 1;
  if (cmp == 0) return 0;
  return -1;
}

static inline s32 ivm_fcmp(float lhs, float rhs) {
  float cmp = lhs - rhs;
  if (cmp > 0.0) return 1;
  if (cmp == 0.0) return 0;
  return -1;
}

static inline s32 ivm_dcmp(double lhs, double rhs) {
  double cmp = lhs - rhs;
  if (cmp > 0.0) return 1;
  if (cmp == 0.0) return 0;
  return -1;
}

static inline s32 ivm_inot(s32 value) { return ~value; }
static inline s32 ivm_bnot(s32 value) { return !value; }
static inline s32 ivm_ineg(s32 value) { return -value; }
static inline s64 ivm_lnot(s64 value) { return ~value; }
static inline s64 ivm_lneg(s64 value) { return -value; }
static inline float ivm_fneg(float value) { return -value; }
static inline double ivm_dneg(double value) { return -value; }

static inline s32 ivm_b2i(s32 value) { return value; }
static inline s32 ivm_s2i(s32 value) { return value; }
static inline s32 ivm_i2b(s32 value) { return value & 0x000000FF; }
static inline s32 ivm_i2s(s32 value) { return value & 0x0000FFFF; }
static inline s64 ivm_i2l(s32 value) { return value; }
static inline float ivm_i2f(s32 value) { return value; }
static inline double ivm_i2d(s32 value) { return value; }
static inline s32 ivm_l2i(s64 value) { return value; }
static inline float ivm_l2f(s64 value) { return value; }
static inline double ivm_l2d(s64 value) { return value; }
static inline s32 ivm_f2i(float value) { return dutch_round_float_int32(value); }
static inline s64 ivm_f2l(float value) { return dutch_round_float_int64(value); }
static inline double ivm_f2d(float value) { return value; }
static inline s32 ivm_d2i(double value) { return dutch_round_double_int32(value); }
static inline s64 ivm_d2l(double value) { return dutch_round_double_int64(value); }
static inline float ivm_d2f(double value) { return value; }

static inline s32 ivm_throw(s32 type, s32 code) { ivm_builtin_throw(type, code); }

static inline void ivm_check(s32 value) { (void)value; }

#endif
